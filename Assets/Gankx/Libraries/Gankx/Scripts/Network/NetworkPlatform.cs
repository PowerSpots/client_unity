namespace Gankx
{
    public enum NetworkPlatform
    {
        None = 0,
        Dogic = 1,
        Wechat = 2,
        QQ = 3,
        WTLogin = 4,
        Guest = 5,
        AutoLogin = 6,

        TencentVideo = 7,

        QR = 0x0100,
        QRWechat,
        QRQQ,

        // Other Platform
        Custom = 0x8000,
        Facebook = 0x8001,
        GooglePlay = 0x8002,
        GooglePlus = 0x8003,
        AppleGameCenter = 0x8004,

        Gmei = 0x8007,		
        App37Wan = 0x8008,
        BeeTalk = 0x8009,
        Kakao = 0x8010,
        Link = 0x8030,		
        Garena = 0x8031,
        Twitter = 0x8032,
        Email = 0x8033,
        Toy = 0x8034,		
        Line = 0x8035,
        Efun = 0x8036,      

        //CoSDK Platform
        CoSDK = 0x8020,
    };
}