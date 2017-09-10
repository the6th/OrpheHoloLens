using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System;
using HoloToolkit.Unity;

#if WINDOWS_UWP
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.UI.Core;
using Orphe;
#endif



namespace Orpe
{
    public class OrpheDeviceManager : Singleton<OrpheDeviceManager>
    {
        public Quaternion deviceQ;

#if WINDOWS_UWP

        bool isConnected = false;
        // delegateの宣言
        public delegate void MyCallback(string msg);
        public delegate void MyValueCallback(OrpheValueChangedEventArgs e);

        // メソッドを格納する変数
        public MyCallback OnDeviceAdded;
        public MyCallback OnDeviceRemoved;
        public MyCallback OnDeviceUpdated;
        public MyCallback OnConnected;
        public MyCallback OnConnectFailed;
        public MyValueCallback OnValueChanged;

        private OrpheShoe _OrpheShoe = null;
        private DeviceWatcher _DeviceWatcher;
        private ObservableCollection<string> _DeviceIdList = new ObservableCollection<string>();



        // Use this for initialization
        public void Init(string deviceName = "orphe")
        {
            Debug.Log("init");
            isConnected = false;
            _DeviceWatcher = DeviceInformation.CreateWatcher(OrpheShoe.GetDeviceSelector(deviceName), new string[] { "System.Devices.Aep.IsConnected", "System.Devices.Aep.SignalStrength", }, DeviceInformationKind.AssociationEndpoint);
            //_DeviceWatcher = DeviceInformation.CreateWatcher("System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\"", null, DeviceInformationKind.AssociationEndpoint);
            _DeviceWatcher.Added += DeviceWatcher_Added;
            _DeviceWatcher.Updated += DeviceWatcher_Updated;
            _DeviceWatcher.Removed += DeviceWatcher_Removed;
            _DeviceWatcher.Start();


        }

        void OnDestroy()
        {
            _DeviceWatcher.Added -= DeviceWatcher_Added;
            _DeviceWatcher.Updated -= DeviceWatcher_Updated;
            _DeviceWatcher.Removed -= DeviceWatcher_Removed;
            _DeviceWatcher.Stop();
            _OrpheShoe?.Disconnect();
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {

            Debug.Log("DeviceWatcher_Added" + args.Id);

            _DeviceIdList.Add(args.Id);
            //   gameObject.SendMessage("IdAddedtoUI", args.Id);

            // Debug.Log("DeviceWatcher_Added:end");
            Task.Run(async () =>
            {
                UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                {
                    OnDeviceAdded(args.Id);
                }, true);

            });

        }

        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            Debug.Log("DeviceWatcher_Updated" + args.Id);


        }

        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {

            Debug.Log("DeviceWatcher_Removed" + args.Id);
            _DeviceIdList.Remove(args.Id);
            //   gameObject.SendMessage("IdRemovedtoUI", args.Id);


            Task.Run(async () =>
            {
                UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                {
                    OnDeviceRemoved(args.Id);
                }, true);

            });

        }


        public async void btnConnect_Click(string deviceID)
        {
            if (isConnected)
            {
                Debug.Log("すでに接続済み");
                return;
            }
                

            _OrpheShoe = new OrpheShoe();
            _OrpheShoe.ValueChanged += OrpheShoe_ValueChanged;

            if (!await _OrpheShoe.Connect(deviceID))
            {
                _OrpheShoe = null;


                Debug.Log("Orpheとの接続確立に失敗しました。\nOrpheをペアリングモードに変更して再接続してください。");
                Task.Run(async () =>
                {
                    UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                    {
                        OnConnectFailed(deviceID);
                    }, true);

                });
                /// await new MessageDialog("Orpheとの接続確立に失敗しました。\nOrpheをペアリングモードに変更して再接続してください。").ShowAsync();
                return;
            }

            _DeviceWatcher.Stop();

            Debug.Log("Connected!");
            Task.Run(async () =>
            {
                UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                {
                    OnConnected(deviceID);
                }, true);

            });


        }

        private void OrpheShoe_ValueChanged(object sender, OrpheValueChangedEventArgs e)
        {
            Debug.Log("OrpheShoe_ValueChanged");
            deviceQ = new Quaternion((float)e.Quaternion.x, (float)e.Quaternion.y, (float)e.Quaternion.z, (float)e.Quaternion.w);
            

            Task.Run(async () =>
            {
                UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                {
                    OnValueChanged(e);
                }, true);

            });
        }


#endif


    }
}
