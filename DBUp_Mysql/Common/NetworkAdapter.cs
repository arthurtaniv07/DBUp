using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;

namespace DBUp_Mysql
{
    //    需要开启 Windows Management Instrumentation服务（默认已经开启），在程序中需要增加 Management引用。

    //主要有NetworkAdapter（保存适配器的IP地址，网关，子网掩码，设置IP方面等 ）,NetworkAdapterUtil(主要是NetworkAdapter类的集合 )两个类。
    //    在windows8 需要在app.manifest文件中


    //   修改配置<requestedExecutionLevel level="requireAdministrator" uiAccess="false" />，不然就算有管理员权限也修改不了IP地址。

    //1 设置IP地址的代码。（设置IP地址，修改IP地址需要管理员权限）

    /// <summary>
    /// 网络适配器类
    /// </summary>
    public class NetworkAdapter
    {
        public string Description { get; set; }
        public string NetworkInterfaceType { get; set; }
        public string NetworkInterfaceID { get; set; }
        public string Speed { get; set; }
        public PhysicalAddress MacAddress { get; set; }
        public GatewayIPAddressInformationCollection Getwaryes { get; set; }
        public UnicastIPAddressInformationCollection IPAddresses { get; set; }
        public IPAddressCollection DhcpServerAddresses { get; set; }
        public bool IsDhcpEnabled { get; set; }
        public IPAddressCollection DnsAddresses { get; set; }



        /// <summary>
        /// 设置IP地址
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="submask"></param>
        /// <param name="getway"></param>
        /// <param name="dns"></param>
        private bool SetIPAddress(string[] ip, string[] submask, string[] getway, string[] dns)
        {
            ManagementClass wmi = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = wmi.GetInstances();
            ManagementBaseObject inPar = null;
            ManagementBaseObject outPar = null;
            string str = "";
            foreach (ManagementObject mo in moc)
            {

                if (!(bool)mo["IPEnabled"])
                    continue;


                if (this.NetworkInterfaceID == mo["SettingID"].ToString())
                {
                    if (ip != null && submask != null)
                    {
                        string caption = mo["Caption"].ToString(); //描述
                        inPar = mo.GetMethodParameters("EnableStatic");
                        inPar["IPAddress"] = ip;
                        inPar["SubnetMask"] = submask;
                        outPar = mo.InvokeMethod("EnableStatic", inPar, null);
                        str = outPar["returnvalue"].ToString();
                        return (str == "0" || str == "1") ? true : false;
                        //获取操作设置IP的返回值， 可根据返回值去确认IP是否设置成功。 0或1表示成功 
                        // 返回值说明网址： https://msdn.microsoft.com/en-us/library/aa393301(v=vs.85).aspx
                    }
                    if (getway != null)
                    {
                        inPar = mo.GetMethodParameters("SetGateways");
                        inPar["DefaultIPGateway"] = getway;
                        outPar = mo.InvokeMethod("SetGateways", inPar, null);
                        str = outPar["returnvalue"].ToString();
                        return (str == "0" || str == "1") ? true : false;
                    }
                    if (dns != null)
                    {
                        inPar = mo.GetMethodParameters("SetDNSServerSearchOrder");
                        inPar["DNSServerSearchOrder"] = dns;
                        outPar = mo.InvokeMethod("SetDNSServerSearchOrder", inPar, null);
                        str = outPar["returnvalue"].ToString();
                        return (str == "0" || str == "1") ? true : false;
                    }

                }
            }
            return false;
        }
        //2 设置IP为自动获取

        /// <summary>
        /// 启用DHCP服务
        /// </summary>
        public void EnableDHCP()
        {

            ManagementClass wmi = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = wmi.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (!(bool)mo["IPEnabled"])
                    continue;

                if (mo["SettingID"].ToString() == this.NetworkInterfaceID) //网卡接口标识是否相等, 相当只设置指定适配器IP地址
                {
                    mo.InvokeMethod("SetDNSServerSearchOrder", null);
                    mo.InvokeMethod("EnableDHCP", null);

                }
            }
        }


        //3.获取所有适配器

        //4  启用， 禁用适配器代码

        /// <summary>
        /// 启用所有适配器
        /// </summary>
        /// <returns></returns>
        public void EnableAllAdapters()
        {
            // ManagementClass wmi = new ManagementClass("Win32_NetworkAdapter");
            // ManagementObjectCollection moc = wmi.GetInstances();
            ManagementObjectSearcher moc = new ManagementObjectSearcher("Select * from Win32_NetworkAdapter where NetEnabled!=null ");
            foreach (System.Management.ManagementObject mo in moc.Get())
            {
                //if (!(bool)mo["NetEnabled"])
                //    continue;
                string capation = mo["Caption"].ToString();
                string descrption = mo["Description"].ToString();
                mo.InvokeMethod("Enable", null);
            }

        }

        /// <summary>
        /// 禁用所有适配器
        /// </summary>
        public void DisableAllAdapters()
        {
            // ManagementClass wmi = new ManagementClass("Win32_NetworkAdapter");
            // ManagementObjectCollection moc = wmi.GetInstances();
            ManagementObjectSearcher moc = new ManagementObjectSearcher("Select * from Win32_NetworkAdapter where NetEnabled!=null ");
            foreach (ManagementObject mo in moc.Get())
            {
                //if ((bool)mo["NetEnabled"])
                //    continue;
                string capation = mo["Caption"].ToString();
                string descrption = mo["Description"].ToString();
                mo.InvokeMethod("Disable", null);
            }

        }
    }

    public class NetworkAdapterUtil
    {


        /// <summary>
        /// 获取所有适配器类型，适配器被禁用则不能获取到
        /// </summary>
        /// <returns></returns>
        public List<NetworkAdapter> GetAllNetworkAdapters() //如果适配器被禁用则不能获取到
        {
            IEnumerable<NetworkInterface> adapters = NetworkInterface.GetAllNetworkInterfaces(); //得到所有适配器
            return GetNetworkAdapters(adapters);

        }
        List<NetworkAdapter> adapterList;
        /// <summary>
        /// 根据条件获取IP地址集合，
        /// </summary>
        /// <param name="adapters">网络接口地址集合</param>
        /// <param name="adapterTypes">网络连接状态，如,UP,DOWN等</param>
        /// <returns></returns>
        private List<NetworkAdapter> GetNetworkAdapters(IEnumerable<NetworkInterface> adapters, params NetworkInterfaceType[] networkInterfaceTypes)
        {
            adapterList = new List<NetworkAdapter>();

            foreach (NetworkInterface adapter in adapters)
            {
                if (networkInterfaceTypes.Length <= 0) //如果没传可选参数，就查询所有
                {
                    if (adapter != null)
                    {
                        NetworkAdapter adp = SetNetworkAdapterValue(adapter);
                        adapterList.Add(adp);
                    }
                    else
                    {
                        return null;
                    }
                }
                else //过滤查询数据
                {
                    foreach (NetworkInterfaceType networkInterfaceType in networkInterfaceTypes)
                    {
                        if (adapter.NetworkInterfaceType.ToString().Equals(networkInterfaceType.ToString()))
                        {
                            adapterList.Add(SetNetworkAdapterValue(adapter));
                            break; //退出当前循环
                        }
                    }
                }
            }
            return adapterList;
        }

        /// <summary>
        /// 设置网络适配器信息
        /// </summary>
        /// <param name="adapter"></param>
        /// <returns></returns>
        private NetworkAdapter SetNetworkAdapterValue(NetworkInterface adapter)
        {
            NetworkAdapter networkAdapter = new NetworkAdapter();
            IPInterfaceProperties ips = adapter.GetIPProperties();
            networkAdapter.Description = adapter.Name;
            networkAdapter.NetworkInterfaceType = adapter.NetworkInterfaceType.ToString();
            networkAdapter.Speed = adapter.Speed / 1000 / 1000 + "MB"; //速度
            networkAdapter.MacAddress = adapter.GetPhysicalAddress(); //物理地址集合
            networkAdapter.NetworkInterfaceID = adapter.Id;//网络适配器标识符

            networkAdapter.Getwaryes = ips.GatewayAddresses; //网关地址集合
            networkAdapter.IPAddresses = ips.UnicastAddresses; //IP地址集合
            networkAdapter.DhcpServerAddresses = ips.DhcpServerAddresses;//DHCP地址集合
            networkAdapter.IsDhcpEnabled = ips.GetIPv4Properties() == null ? false : ips.GetIPv4Properties().IsDhcpEnabled; //是否启用DHCP服务

            IPInterfaceProperties adapterProperties = adapter.GetIPProperties();//获取IPInterfaceProperties实例  
            networkAdapter.DnsAddresses = adapterProperties.DnsAddresses; //获取并显示DNS服务器IP地址信息 集合
            return networkAdapter;
        }
    }
}
