abstract class AbstractGame
{
	// Used for when the game is loaded to identify which players
	// were previously in the game. New game => empty list
	// The arraylist of players should be in the turn order. ie once
	// the game starts it is put into order.

	// The assumption is that this is created when a lobby is made
	// It should probably have a map and other things as well
	ArrayList<Player> players { get; };
	int roundsPlayed { get; };
}