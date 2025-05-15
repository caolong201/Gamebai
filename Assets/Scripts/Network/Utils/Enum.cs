namespace Suni.Enum
{
    public enum ENetworkHeader
    {
        Success = 0,
        Login = 1,
        LoginGuest = 2,
        Register = 4,
        RecieveMyInfo = 7,
        JoinPhomGame = 8,
        EnterGame = 2000,
        OtherPlayerReady = 2001,
        PlayerLeft = 2002,
        SelfReady = 2004,
        StartGame = 2005,
        StartGame2 = 2006,
        PlayCard = 2007,
        DrawFromDiscard = 2008,// An bai
        DrawFromDeck = 2009,
        Result = 2010,
        DropPhom = 2011,
        GuiBai = 2012,
        CreatePrivateTable = 2013,
        JoinPrivateTable =2014,
        Chatcontent= 2015,
       
    }
    
    public enum EGameType
    {
        None = 0,
        DOG = 1,
        TIEN_LEN,
        CO_UP,
        CO_TUONG,
        POKER,
        BA_CAY,
        MAU_BINH,
        XI_TO,
        XOC_DIA = 9,
        PHOM
    }
}