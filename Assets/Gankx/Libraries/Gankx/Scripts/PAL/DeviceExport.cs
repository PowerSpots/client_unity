using UnityEngine;

public class DeviceExport
{
    public static string GetHardware()
    {
        return SystemInfo.deviceModel;
    }

    public static string GetTelecomOpera()
    {
        //IApolloNetworkService network = IApollo.Instance.GetService(ServiceType.Network) as IApolloNetworkService;
        //return network.GetDetailNetworkInfo().CarrierCode;
        return "TODO";
    }

    public static string GetNetworkState()
    {
        //IApolloNetworkService network = IApollo.Instance.GetService(ServiceType.Network) as IApolloNetworkService;
        //return network.GetDetailNetworkInfo().DetailState.ToString();
        return "TODO";
    }

    public static int GetRegisterChannelId()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        return 0;
#else
        IApolloCommonService common = IApollo.Instance.GetService(Apollo.Plugins.Msdk.ApolloServiceType.Common) as IApolloCommonService;
        int channel = 0;
        if(common!=null)
        {
            int.TryParse(common.GetRegisterChannelId(), out channel);

        }
        return channel;
#endif
    }

    public static int GetPackageChannelId()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        return 0;
#else
        IApolloCommonService common = IApollo.Instance.GetService(Apollo.Plugins.Msdk.ApolloServiceType.Common) as IApolloCommonService;
        int channel = 0;
        if(common!=null)
        {
            int.TryParse(common.GetChannelId(), out channel);

        }
        return channel;
#endif
    }

    public static string GetSoftware()
    {
        return SystemInfo.operatingSystem;
    }

    public static int GetScreenWidth()
    {
        return Screen.width;
    }

    public static int GetScreenHeight()
    {
        return Screen.height;
    }

    public static int GetScreenDensity()
    {
        return (int)Screen.dpi;
    }

    public static string GetCPUHardware()
    {
        return string.Format("{0}_{1}_{2}", SystemInfo.processorType,SystemInfo.processorFrequency,SystemInfo.processorCount); ;
    }

    public static string GetGLRender()
    {
        return SystemInfo.graphicsDeviceName;
    }

    public static string GetGLVersion()
    {
        return SystemInfo.graphicsDeviceVersion;
    }

    public static string GetDeviceId()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }

    /* message C2G_LOGIN
{
	required fixed32 tag				= 1;
	required int32 version				= 2;
	required int32 server_group			= 3;
	optional uint32 msg_crc				= 4;
	optional uint64 roleid				= 5;
	optional bool is_query              = 6;

	optional bool open_relay            = 7 [default = false];
    optional int32 c2g_proto_seq		= 8;
	optional int32 g2c_proto_seq		= 9;

	optional int32 platform				= 10;
	optional string hardware			= 11;
	optional string telelcom_oper		= 12;
	optional string network				= 13;
	optional int32 channel				= 14;
	optional string software			= 15;	
	optional int32	screen_width		= 16;
	optional int32	screen_hight		= 17;
	optional int32	density				= 18;
	optional string cpu_hardware		= 19;
	optional int32	memory				= 20;
	optional string gl_render			= 21;
	optional string gl_version			= 22;
	optional string device_id			= 23;
}
*/
}
 