using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tarneeb.Models
{
    /// <summary>
    /// This class encapsulates all Tarneeb game logic and contains the game state
    /// </summary>
    public class GameSession
    {
        #region General Game Session Properties
       
        /// <summary>
        /// The identifier of the game
        /// </summary>
        public Guid Id { get; set; }


        /// <summary>
        /// The time in which the game session was opened for players
        /// </summary>
        public DateTime CreationTime { get; set; }

        private Dictionary<PlayerPosition, Player> Players
        {
            get
            {
                var players = new Dictionary<PlayerPosition, Player>();
                players.Add(
                    PlayerPosition.North,
                    GetPlayer(PlayerPosition.North));
                players.Add(
                    PlayerPosition.South,
                    GetPlayer(PlayerPosition.South));
                players.Add(
                    PlayerPosition.East,
                    GetPlayer(PlayerPosition.East));
                players.Add(
                    PlayerPosition.West,
                    GetPlayer(PlayerPosition.West));

                return players;
            }
        }

        private Dictionary<TeamPosition, Team> teams;

        #endregion

        #region Game Session State Properties
        private GameSessionStatus status;

        /// <summary>
        /// Represents game status, whether it's Waiting, Bidding, Playing, or Finished
        /// The property setter changes the status and prepares the session state
        /// for the new state
        /// </summary>
        public GameSessionStatus Status
        {
            get { return status; }
            set
            {
                status = value;
                switch (status)
                {
                    case GameSessionStatus.Bidding:
                        if (bidding == null)
                        {
                            bidding = new Bidding(this, currentBiddingStartPosition);
                        }
                        break;
                    case GameSessionStatus.GamePlay:
                        if (gamePlay == null)
                        {
                            gamePlay = new GamePlay(this, GetPlayerPosition(CurrentBid.Player), CurrentBid.Suit);
                        }
                        break;

                }
            }
        }
        #endregion

        #region Game Session Bidding related properties

        public Bid CurrentBid { get; set; }
        private PlayerPosition currentBiddingStartPosition;

        #endregion

        #region General current turn property for both bidding and game play
        public PlayerPosition? CurrentTurn
        {
            get
            {
                switch (Status)
                {
                    case GameSessionStatus.Bidding:
                        return bidding.CurrentTurn;
                    case GameSessionStatus.GamePlay:
                        return gamePlay.CurrentTurn;
                    default:
                        return null;
                }
            }
        }
        #endregion

        #region Internal sub-module references
        private GamePlay gamePlay;
        private Bidding bidding;
        #endregion

        #region Score Properties
        public MatchScore MatchScore { get; set; }
        #endregion

        public GameSession()
        {
            Id = Guid.NewGuid();
            CreationTime = DateTime.Now;
            Status = GameSessionStatus.WaitingForPlayers;            

            teams = new Dictionary<TeamPosition, Team>(2);
            teams.Add(TeamPosition.NorthSouth, new Team());
            teams.Add(TeamPosition.EastWest, new Team());

            //initialize bidding start position to the one to the right of South
            currentBiddingStartPosition = PlayerPosition.East;

            MatchScore = new MatchScore();
        }

        #region Join / Leave methods in the WaitingForPlayers state
        /// <summary>
        /// Make the specified player join the current game session on the 
        /// specified position (North, South, East, West)
        /// </summary>
        public void Join(Player player, PlayerPosition position)
        {
            if (Status == GameSessionStatus.WaitingForPlayers)
            {
                Team team = null;
                switch (position)
                {
                    case PlayerPosition.North:
                    case PlayerPosition.South:
                        team = teams[TeamPosition.NorthSouth];
                        break;
                    case PlayerPosition.East:
                    case PlayerPosition.West:
                        team = teams[TeamPosition.EastWest];
                        break;
                }

                team.Join(player, position);

                if (teams[TeamPosition.NorthSouth].IsReady && teams[TeamPosition.EastWest].IsReady)
                {
                    Status = GameSessionStatus.Bidding;
                }
            }
            else
            {
                //TODO: throw exception that the game cannot accept new members
            }
        }

        /// <summary>
        /// Make the specified player leave the current game session
        /// </summary>
        public void Leave(Player player)
        {
            if (teams[TeamPosition.NorthSouth].IsMember(player))
            {
                teams[TeamPosition.NorthSouth].Leave(player);
            }
            else if (teams[TeamPosition.EastWest].IsMember(player))
            {
                teams[TeamPosition.EastWest].Leave(player);
            }
            else
            {
                //TODO: throw an exception that the player is not enrolled in the game
            }
        }

        #endregion

        #region Bidding Methods
        /// <summary>
        /// In the bidding phase, places the specified bid for the current
        /// turn player
        /// </summary>
        public void PlaceBid(Bid bid)
        {
            if (Status == GameSessionStatus.Bidding && bidding != null)
            {
                bidding.PlaceBid(bid);
            }
        }

        /// <summary>
        /// Returns the current state of the auction including the bids list
        /// , is closed, and current turn
        /// </summary>
        /// <returns></returns>
        public BiddingState GetGameBiddingState()
        {
            if (Status == GameSessionStatus.Bidding && bidding != null)
            {
                return new BiddingState()
                {
                    Bids = bidding.Bids,
                    CurrentTurn = bidding.CurrentTurn,
                    IsClosed = bidding.IsClosed,
                    Winner = (bidding.Winner != null) ?
                        (PlayerPosition?)GetPlayerPosition(bidding.Winner.Player) : null,
                    WinnerBid = bidding.Winner
                };
            }
            else
            {
                //throw new InvalidOperationException("Game is not in the Bidding state");
                return new BiddingState()
                {
                    IsClosed = true,
                    Winner = (CurrentBid != null) ?
                        (PlayerPosition?)GetPlayerPosition(CurrentBid.Player) : null,
                    WinnerBid = CurrentBid
                };
            }
        }

        /// <summary>
        /// Callback method to be called by the bidding object upon the completion
        /// of the bidding process. This method declares the winning bid and moves
        /// the state to the next state
        /// </summary>
        internal void BiddingComplete()
        {
            this.CurrentBid = bidding.Winner;
            Status = GameSessionStatus.GamePlay;
            bidding = null;
        }

        #endregion

        #region Game Play Methods
        /// <summary>
        /// Makes the current turn player play the specified card
        /// </summary>
        public void PlaceCard(Player player,Card card)
        {
            if (Status == GameSessionStatus.GamePlay && gamePlay != null)
            {
                gamePlay.PlaceCard(player, card);
            }
        }

        /// <summary>
        /// Return the current state of the game play including
        /// number of tricks won by each team, number of cards held by each player
        /// , the current player cards, the cards in the current trick
        /// </summary>
        /// <returns></returns>
        public GamePlayState GetGamePlayState(Player player)
        {
            if (Status == GameSessionStatus.GamePlay)
            {
                if (gamePlay != null)
                {
                    
                    PlayerPosition position = GetPlayerPosition(player);

                    return new GamePlayState()
                    {
                        CurrentCards = gamePlay.PlayerCards[position],
                        CurrentTurn = gamePlay.CurrentTurn,
                        CurrentTrick = gamePlay.CurrentTrick,
                        CurrentTrickBaseSuit = gamePlay.CurrentTrickBaseSuit,
                        TricksWon = gamePlay.TricksWon
                    };
                }
            }
            else
            {
                throw new InvalidOperationException("Game is not in the Game Play state");
            }

            return null;
        }

        /// <summary>
        /// Callback method to be called from the game play object upon 
        /// completion of the game. This method calculates the score and 
        /// updates the match score and moves to the next state
        /// </summary>
        internal void GameComplete()
        {
            TeamPosition bidTeamPosition =
                GetTeamPosition(GetPlayerPosition(CurrentBid.Player));
            TeamPosition otherTeamPosition =
                (bidTeamPosition == TeamPosition.NorthSouth) ? TeamPosition.EastWest : TeamPosition.NorthSouth;

            int bidTeamTricks = gamePlay.TricksWon[bidTeamPosition];
            int otherTeamTricks = 13 - bidTeamTricks;
            int bidTeamScore = 0;
            int otherTeamScore = 0;

            if (bidTeamTricks >= CurrentBid.Tricks + 6)
            {
                bidTeamScore = bidTeamTricks;
            }
            else
            {
                bidTeamScore -= (CurrentBid.Tricks + 6);
                otherTeamScore = otherTeamTricks;
            }
            //TODO: handle double and re-double
            int nsScore = (bidTeamPosition == TeamPosition.NorthSouth) ? bidTeamScore : otherTeamScore;
            int ewScore = (bidTeamPosition == TeamPosition.EastWest) ? bidTeamScore : otherTeamScore;

            MatchScore.AddGameScore(new GameScore(CurrentBid,nsScore, ewScore));
            if (MatchScore.Winner != null)
            {
                //Match completed
                Status = GameSessionStatus.Score;
            }
            else
            {
                Status = GameSessionStatus.Bidding;
            }
        }
        #endregion

        #region Game Session State
        /// <summary>
        /// Returns the current state of the game session, including Players and Is Waiting
        /// </summary>
        /// <returns></returns>
        public GameSessionState GetGameSessionState()
        {
            return new GameSessionState()
            {
                Id=this.Id,
                Players=Players,
                CreationTime=CreationTime,
                Status=Status
            };
        }

        #endregion

        #region General Helper methods
        /// <summary>
        /// Get the player on the specified position
        /// </summary>
        public Player GetPlayer(PlayerPosition position)
        {
            switch (position)
            {
                case PlayerPosition.North:
                    return teams[TeamPosition.NorthSouth].Player1;
                case PlayerPosition.South:
                    return teams[TeamPosition.NorthSouth].Player2;
                case PlayerPosition.East:
                    return teams[TeamPosition.EastWest].Player1;
                case PlayerPosition.West:
                    return teams[TeamPosition.EastWest].Player2;
            }

            return null;
        }

        /// <summary>
        /// Get the team position of the specified player position
        /// </summary>
        public TeamPosition GetTeamPosition(PlayerPosition position)
        {
            switch (position)
            {
                case PlayerPosition.North:
                case PlayerPosition.South:
                    return TeamPosition.NorthSouth;
                case PlayerPosition.East:
                case PlayerPosition.West:
                    return TeamPosition.EastWest;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Get the position of the specified player
        /// </summary>
        public PlayerPosition GetPlayerPosition(Player player)
        {
            return 
                Players
                .Where(p => p.Value == player)
                .Select(p => p.Key).FirstOrDefault();
        }
        #endregion

    }

    public enum GameSessionStatus
    {
        WaitingForPlayers,
        Bidding,
        GamePlay,
        Score
    }
}