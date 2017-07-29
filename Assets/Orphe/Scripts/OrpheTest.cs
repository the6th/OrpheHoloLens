using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Windows.Devices.Enumeration;
using System.Collections.ObjectModel;
using Windows.Devices.Bluetooth;
using System;

#if WINDOWS_UWP
using Orphe;
#endif

public class OrpheTest : MonoBehaviour
{

    bool isPrepare = false;
#if WINDOWS_UWP
    private OrpheShoe _OrpheShoe = null;
    private DeviceWatcher _DeviceWatcher;
    private ObservableCollection<string> _DeviceIdList = new ObservableCollection<string>();

    

    // Use this for initialization
    void Start()
    {
        _DeviceWatcher = DeviceInformation.CreateWatcher("System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\"", null, DeviceInformationKind.AssociationEndpoint);
        _DeviceWatcher.Added += DeviceWatcher_Added;
        _DeviceWatcher.Updated += DeviceWatcher_Updated;
        _DeviceWatcher.Removed += DeviceWatcher_Removed;
        _DeviceWatcher.Start();
    }

    void OnDestroy()
    {
        _DeviceWatcher.Stop();
        _OrpheShoe?.Disconnect();
    }

    private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
    {
        Debug.Log("DeviceWatcher_Added" + args.Id);

        _DeviceIdList.Add(args.Id);

        if(isPrepare)
        {
            btnConnect_Click(args.Id);
        }
        /*
        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        {
            _DeviceIdList.Add(args.Id);
        });
        */
    }

    private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
    {
        Debug.Log("DeviceWatcher_Updated" + args.Id);
    }

    private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
    {
        Debug.Log("DeviceWatcher_Removed" +args.Id);
        _DeviceIdList.Remove(args.Id);
        /*
        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        {

            _DeviceIdList.Remove(args.Id);
        });
        */
    }


    private async void btnConnect_Click(string deviceID)
    {
       // if (lstDeviceIdList.SelectedIndex < 0) return;

        Debug.Log("OrpheShoe.ServiceUuid:" + OrpheShoe.ServiceUuid);
        Debug.Log("lstDeviceIdList.SelectedItem" + deviceID);

        var device = await BluetoothLEDevice.FromIdAsync(deviceID);

        var service = device.GetGattService(OrpheShoe.ServiceUuid);

        _OrpheShoe = new OrpheShoe();
        _OrpheShoe.ValueChanged += OrpheShoe_ValueChanged;
        _OrpheShoe.Connect(service);

        Debug.Log("Connected!!!!!!!!!!!!!!!!!");
        /*
        lstDeviceIdList.IsEnabled = false;
        btnConnect.IsEnabled = false;
        btnScene.IsEnabled = true;
        btnLight.IsEnabled = true;
        */
    }

    private void OrpheShoe_ValueChanged(object sender, OrpheValueChangedEventArgs e)
    {
        var now = DateTime.Now;

        //Debug.LogFormat
        Debug.LogFormat("{0:HHmmssfff} {1:f3} {2:f3} {3:f3}", now, e.Quaternion.x, e.Quaternion.y, e.Quaternion.z);
    }


#endif

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("isPrepared");
            isPrepare = true;
        }
    }
}
