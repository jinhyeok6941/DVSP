using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Ports;
using System.Text;

public class SerialTest_JH : MonoBehaviour
{
    public RobotConnector2 r;


        //public int baudrate = 57600;
        //private SerialPort _serialPort;
        // public List<string> portNames = new List<string>(); // 탐색한 포트들
        //public string portName; // 연결할 포트명

        public EventHandler OnConnected;
        public EventHandler OnConnectionFailed;
        public EventHandler OnDisconnected;
        public EventHandler OnSearchCompleted;
        public EventHandler OnUpdated;

        public Protocol.State state = new Protocol.State();
        public Protocol.Attitude attitude = new Protocol.Attitude();
        public Protocol.Altitude altitude = new Protocol.Altitude();
        public Protocol.Motion motion = new Protocol.Motion();
        public Protocol.Flow flow = new Protocol.Flow();
        public Protocol.Range range = new Protocol.Range();
        public Protocol.Position position = new Protocol.Position();
        public Protocol.sendPacket.packetTerm term = new Protocol.sendPacket.packetTerm();
        public Protocol.Information information = new Protocol.Information();
        public Protocol.Trim trim = new Protocol.Trim();
        public Control_2.Quad8 quad8 = new Control_2.Quad8();
        public Protocol.Count count = new Protocol.Count();


        public int takeoffPressed = 0;
        public int landingPressed = 0;
        public int stopPressed = 0;
        public int trimPressed = 0;
        public int clearbiasPressed = 0;
        public int SpeedPressed = 0;
        public int _nSpeed = 1;

        //    public int  mode_linkPressed = 0;
        //    public int  mode_controlPressed = 0;

        public int _nSendCount_VerRC = 0; // 조종기 버전 요청 연속 전송횟수
        public int _nSendCount_VerRFModule = 0; // RF모듈 버전 요청 연속 전송횟수
        public int _nSendCount_VerDrone = 0; // 드론 버전 요청 연속 전송횟수



        // Information (펌웨어 버젼)-----------------------------------------------------------------------------------------
        public uint _nModelNumber = 0; // 받아온 데이터

        ushort _usVersion_Build = 0; // 받아온 데이터
        byte _byVersion_Minor = 0x00;
        byte _byVersion_Major = 0x00;

        ushort _usDate_Year = 0; // 받아온 데이터
        byte _byDate_Month = 0x00;
        byte _byDate_Day = 0x00;


        public ushort _nC_Version_build = 0;  // UI 표시용 - 컨트롤러 빌드 버전
        public byte _nC_Version_major = 0; // UI 표시용 - 컨트롤러 주 버전
        public byte _nC_Version_minor = 0; // UI 표시용 - 컨트롤러 부 버전
        public ushort _nD_Version_build = 0;  // UI 표시용 - 드론 빌드 버전
        public byte _nD_Version_major = 0; // UI 표시용 - 드론 주 버전
        public byte _nD_Version_minor = 0; // UI 표시용 - 드론 부 버전




        public int trimRoll = 0;
        public int trimPitch = 0;
        public int trimYaw = 0;
        public int trimThrottle = 0;

        public byte lightRed = 0;
        public byte lightGreen = 0;
        public byte lightBlue = 0;
        public int lightSend = 0;

        public int Flip_Front = 0; // 플립 앞
        public int Flip_Back = 0; // 플립 뒤
        public int Flip_Left = 0; // 플립 왼쪽
        public int Flip_Right = 0; // 플립 오른쪽

        public int Count = 0; // 비행 관련 카운트 값 (이착륙 충돌 횟수)



        public bool _opened = false; // 포트 열림
        public bool _connected = false; // 실제 드론 접속
        private int _sendCounter = 0;
        //private ulong _gCounter = 0;

        float _fSendInterval = 0.02f; // packetSendingHandler() 함수 인보크 재실행 간격



        // 각 센서 정밀값 활성 (RobotConnector2용)
        public bool _bIsDetailImageFlow = false;
        public bool _bIsDetailRange = false;
        public bool _bIsDetailAxis = false;
        public bool _bIsDetailSensor = false;

        public bool _bIsAllSensor = true; // 모든 센서 균일하게 작동







        // Awake ----------------------------------------------------------------------------------------------
        void Awake()
        {
            //_serialPort = new SerialPort();
            //_serialPort.DtrEnable = false;
            //_serialPort.RtsEnable = false;
            //_serialPort.DataBits = 8;
            //_serialPort.ReadTimeout = 1;
            //_serialPort.WriteTimeout = 1000;
            //_serialPort.Parity = Parity.None;
            //_serialPort.StopBits = StopBits.One;

            //ResetData();

            //Invoke("packetSendingHandler", 0.05f);
        }

        // 버전 받기 시작 ----------------------------------------------------------------------------------------------------------------
        void GetVersion_Start()
        {
            _nSendCount_VerRC = 10;
            _nSendCount_VerRFModule = 10;
            _nSendCount_VerDrone = 10;

            if ((_nC_Version_major != 0) && (_nD_Version_major != 0)) // 조종기, 드론 버전을 모두 받았다면
            {
                Debug.Log("Got Version");
                CancelInvoke("GetVersion_Start");
            }
            else
                Invoke("GetVersion_Start", 2f);
        }




        // 버젼 받기 -------------------------------------------------------------------------------------------------------------------
        void GetVersion(uint p_Model, byte p_Major, byte p_Minor, ushort p_Build)
        {
            string serController = "";

            if ((p_Model >= 270338) && (p_Model <= 270350)) // 코드론 II 조종기
            {
                _nC_Version_build = p_Build;
                _nC_Version_major = p_Major;
                _nC_Version_minor = p_Minor;
            }
            else if ((p_Model >= 204803) && (p_Model <= 204810)) // 코드론 미니 조종기
            {
                _nC_Version_build = p_Build;
                _nC_Version_major = p_Major;
                _nC_Version_minor = p_Minor;
            }
            else if ((p_Model >= 208896) && (p_Model <= 208910)) // RF 모듈
            {
                _nC_Version_build = p_Build;
                _nC_Version_major = p_Major;
                _nC_Version_minor = p_Minor;
            }
            else if ((p_Model >= 266245) && (p_Model <= 266260)) // 코드론 II
            {
                _nD_Version_build = p_Build;
                _nD_Version_major = p_Major;
                _nD_Version_minor = p_Minor;
            }
            else // 코드론 미니
            {
                _nD_Version_build = p_Build;
                _nD_Version_major = p_Major;
                _nD_Version_minor = p_Minor;
            }

            if ((_nC_Version_major != 0) && (_nD_Version_major != 0))
            {


            }
        }


        // Update ------------------------------------------------------------------------------------------------------------
        void Update()
        {
            if (_opened == true)
            {
            
            }
        }

    //    void Start ()
    //    {
    //        byte[] tempBuff = { 0x40, 0x08, 0x10, 0x70, 0x03, 0x10, 0x10, 0x01, 0x02, 0x02, 0x01, 0x64 };
    //        byte crcL, crcH;
    //        ushort crc = crc16_ccitt(tempBuff, 0, tempBuff.Length);    //0A 55 crc crc 는 제외함
    //        crcL = (byte)(crc & 0xFF);
    //        crcH = (byte)((crc & 0xFF00) >> 8);
    //
    //        Debug.Log(crcL + "     " + crcH);
    //    }







    public void OnClickPortSearch()
    {
        //포트 서치 및 지정
        //r.portNames.Clear();
        r.portNames.AddRange(SerialPort.GetPortNames());
        r.portName = r.portNames[0];
        //접속 
        r.Connect();
    }


    public void OnClickTakeOff()
    {
        //r.landingPressed++;
        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x80, 0x10, 0x07, 0x11, 0x7B, 0x1E };  // 이륙
        r._serialPort.Write(packetBuffer, 0, packetBuffer.Length);
    }

    public void OnClickLanding()
    {
        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x80, 0x10, 0x07, 0x12, 0x18, 0x2E };
        r._serialPort.Write(packetBuffer, 0, packetBuffer.Length);
    }






    // 현재 안씀 ##########################################################################################

    // 포트 서치 ----------------------------------------------------------------------------------------------------------

    // PortSearch


    // 선택한 포트로 연결 --------------------------------------------------------------------------------------------------------------



    // Read ----------------------------------------------------------------------------------------------------------------------



    // ErrorDisconnect ----------------------------------------------------------------------------------------------------------------



    // Disconnect -----------------------------------------------------------------------------------------------------------------------




    // crc16_ccitt -------------------------------------------------------------------------------------------------------------------------
    public ushort crc16_ccitt(byte[] buf, int start, int len)
        {
            ushort[] crc16tab = new ushort[] { 0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50a5, 0x60c6, 0x70e7, 0x8108, 0x9129, 0xa14a, 0xb16b, 0xc18c, 0xd1ad, 0xe1ce, 0xf1ef, 0x1231, 0x0210, 0x3273, 0x2252, 0x52b5, 0x4294, 0x72f7, 0x62d6, 0x9339, 0x8318, 0xb37b, 0xa35a, 0xd3bd, 0xc39c, 0xf3ff, 0xe3de, 0x2462, 0x3443, 0x0420, 0x1401, 0x64e6, 0x74c7, 0x44a4, 0x5485, 0xa56a, 0xb54b, 0x8528, 0x9509, 0xe5ee, 0xf5cf, 0xc5ac, 0xd58d, 0x3653, 0x2672, 0x1611, 0x0630, 0x76d7, 0x66f6, 0x5695, 0x46b4, 0xb75b, 0xa77a, 0x9719, 0x8738, 0xf7df, 0xe7fe, 0xd79d, 0xc7bc, 0x48c4, 0x58e5, 0x6886, 0x78a7, 0x0840, 0x1861, 0x2802, 0x3823, 0xc9cc, 0xd9ed, 0xe98e, 0xf9af, 0x8948, 0x9969, 0xa90a, 0xb92b, 0x5af5, 0x4ad4, 0x7ab7, 0x6a96, 0x1a71, 0x0a50, 0x3a33, 0x2a12, 0xdbfd, 0xcbdc, 0xfbbf, 0xeb9e, 0x9b79, 0x8b58, 0xbb3b, 0xab1a, 0x6ca6, 0x7c87, 0x4ce4, 0x5cc5, 0x2c22, 0x3c03, 0x0c60, 0x1c41, 0xedae, 0xfd8f, 0xcdec, 0xddcd, 0xad2a, 0xbd0b, 0x8d68, 0x9d49, 0x7e97, 0x6eb6, 0x5ed5, 0x4ef4, 0x3e13, 0x2e32, 0x1e51, 0x0e70, 0xff9f, 0xefbe, 0xdfdd, 0xcffc, 0xbf1b, 0xaf3a, 0x9f59, 0x8f78, 0x9188, 0x81a9, 0xb1ca, 0xa1eb, 0xd10c, 0xc12d, 0xf14e, 0xe16f, 0x1080, 0x00a1, 0x30c2, 0x20e3, 0x5004, 0x4025, 0x7046, 0x6067, 0x83b9, 0x9398, 0xa3fb, 0xb3da, 0xc33d, 0xd31c, 0xe37f, 0xf35e, 0x02b1, 0x1290, 0x22f3, 0x32d2, 0x4235, 0x5214, 0x6277, 0x7256, 0xb5ea, 0xa5cb, 0x95a8, 0x8589, 0xf56e, 0xe54f, 0xd52c, 0xc50d, 0x34e2, 0x24c3, 0x14a0, 0x0481, 0x7466, 0x6447, 0x5424, 0x4405, 0xa7db, 0xb7fa, 0x8799, 0x97b8, 0xe75f, 0xf77e, 0xc71d, 0xd73c, 0x26d3, 0x36f2, 0x0691, 0x16b0, 0x6657, 0x7676, 0x4615, 0x5634, 0xd94c, 0xc96d, 0xf90e, 0xe92f, 0x99c8, 0x89e9, 0xb98a, 0xa9ab, 0x5844, 0x4865, 0x7806, 0x6827, 0x18c0, 0x08e1, 0x3882, 0x28a3, 0xcb7d, 0xdb5c, 0xeb3f, 0xfb1e, 0x8bf9, 0x9bd8, 0xabbb, 0xbb9a, 0x4a75, 0x5a54, 0x6a37, 0x7a16, 0x0af1, 0x1ad0, 0x2ab3, 0x3a92, 0xfd2e, 0xed0f, 0xdd6c, 0xcd4d, 0xbdaa, 0xad8b, 0x9de8, 0x8dc9, 0x7c26, 0x6c07, 0x5c64, 0x4c45, 0x3ca2, 0x2c83, 0x1ce0, 0x0cc1, 0xef1f, 0xff3e, 0xcf5d, 0xdf7c, 0xaf9b, 0xbfba, 0x8fd9, 0x9ff8, 0x6e17, 0x7e36, 0x4e55, 0x5e74, 0x2e93, 0x3eb2, 0x0ed1, 0x1ef0 };
            ushort crc = 0;
            for (int counter = start; counter < start + len; counter++) crc = (ushort)(crc << 8 ^ crc16tab[crc >> 8 ^ (ushort)buf[counter] & 0x00ff]);
            return crc;
        }







        // 데이터 초기화 ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        void ResetData()
        {
            // Information (펌웨어 버젼)-----------------------------------------------------------------------------------------
            _nModelNumber = 0; // 받아온 데이터

            _usVersion_Build = 0; // 받아온 데이터
            _byVersion_Minor = 0x00;
            _byVersion_Major = 0x00;

            _usDate_Year = 0; // 받아온 데이터
            _byDate_Month = 0x00;
            _byDate_Day = 0x00;

            _nC_Version_build = 0;  // 컨트롤러 빌드 버전
            _nC_Version_major = 0; // 컨트롤러 주 버전
            _nC_Version_minor = 0; // 컨트롤러 부 버전
            _nD_Version_build = 0;  // 드론 빌드 버전
            _nD_Version_major = 0; // 드론 주 버전
            _nD_Version_minor = 0; // 드론 부 버전
        }







    }// RobotConnector =============================================================================================================================================================
