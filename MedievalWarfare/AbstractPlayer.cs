enum PlayerStatus
{
	Offline,
	Online
}

abstract class AbstractPlayer
{
	string username { get; };
	string password { get; };
	PlayerStatus status { get; };
	int wins { get; };
	int losses { get; };
}