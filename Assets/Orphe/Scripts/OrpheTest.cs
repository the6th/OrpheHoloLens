using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System;

#if WINDOWS_UWP
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.UI.Core;
using Orphe;
#endif



namespace Orpe
{
    public class OrpheTest : MonoBehaviour
    {
        public GameObject UITextPrefab;
        bool isInit = false;

#if WINDOWS_UWP
        private void Start()
        {
            //  OrpheDeviceManager.Instance.addCallback(TestCallBack);
            OrpheDeviceManager.Instance.Init();
            OrpheDeviceManager.Instance.OnDeviceAdded += IdAddedtoUI;
            OrpheDeviceManager.Instance.OnDeviceRemoved += IdRemovedtoUI;

            OrpheDeviceManager.Instance.OnValueChanged += ValueChanged;

        }


        private void ValueChanged(OrpheValueChangedEventArgs e)
        {
            Debug.Log(e.Quaternion);
        }
#endif
        private void TestCallBack(string msg)
        {
            Debug.Log("TestCallBack:done:" + msg);
        }

        // Update is called once per frame
        void Update()
        {
            // Debug.Log("Update");

            isInit = true;
            if (Input.GetKeyUp(KeyCode.Space))
            {
                //   Debug.Log("isPrepared");
                //isPrepare = true;
                IdAddedtoUI("hoge");
            }
        }
        public void IdAddedtoUI(string text)
        {
            Debug.Log("IdAddedtoUI" + text);
            if (UITextPrefab == null)
            {
                Debug.Log("UITextPrefab not found");
                return;
            }

            GameObject g = GameObject.Instantiate(UITextPrefab);
            g.transform.SetParent(transform);
            g.name = text;
            g.transform.localPosition = new Vector3(0, transform.childCount * -0.3f, 0);
            g.GetComponent<TextMesh>().text = text;

        }
        public void IdRemovedtoUI(string text)
        {
            Debug.Log("IdRemovedtoUI" + text);
            Destroy(transform.Find(text).gameObject);

        }

        public void IdClicked(string id)
        {
            Debug.Log("IdClicked:" + id);
#if WINDOWS_UWP
            OrpheDeviceManager.Instance.btnConnect_Click(id);
#endif
        }
    }
}
