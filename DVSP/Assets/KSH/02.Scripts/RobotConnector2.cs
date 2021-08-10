using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Ports;
using System.Text;


// ===========================================

// 로봇 컨넥터 (코드론2 전용) 오창연, 최태호

// ===========================================




// Protocol #########################################################################################################################################
namespace Protocol
{
    // sendPacket ####################################
    namespace sendPacket
    {
        enum Type
        {
            control                         =0x00,
            state,
            attitude,
            altitude,
            motion,
            flow,
            takeoff,
            landing,
            stop,
            light,

            EndOfType
        }
    }
    // DataType ####################################
    namespace DataType
    {
        enum Type
        {
            None                        = 0x00,     // 없음
            Ping                        = 0x01,     // 통신 확인
            Ack                         = 0x02,     // 데이터 수신에 대한 응답
            Error                       = 0x03,     // 오류(reserve, 비트 플래그는 추후에 지정)
            Request                     = 0x04,     // 지정한 타입의 데이터 요청
            Message                     = 0x05,     // 문자열 데이터
            Address                     = 0x06,     // 장치 주소(MAC이 있는 경우 MAC) 혹은 고유번호(MAC이 없는 경우 UUID)
            Information                 = 0x07,     // 펌웨어 및 장치 정보
            Update                      = 0x08,     // 펌웨어 업데이트
            UpdateLocation              = 0x09,     // 펌웨어 업데이트 위치 정정
            Encrypt                     = 0x0A,     // 펌웨어 암호화
            SystemCount                 = 0x0B,     // 시스템 카운트
            SystemInformation           = 0x0C,     // 시스템 정보
            Registration                = 0x0D,     // 제품 등록(암호화 데이터 및 등록 데이터를 데이터 길이로 구분)
            Administrator               = 0x0E,     // 관리자 권한 획득
            Monitor                     = 0x0F,     // 디버깅용 값 배열 전송. 첫번째 바이트에 타입, 두 번째 바이트에 페이지 지정(수신 받는 데이터의 저장 경로 구분)
            Control                     = 0x10,     // 조종

            Command                     = 0x11,     // 명령
            Pairing                     = 0x12,     // 페어링
            Rssi                        = 0x13,     // RSSI

            // Light
            LightManual                 = 0x20,     // LED 수동 제어
            LightMode                   = 0x21,     // LED 모드
            LightEvent                  = 0x22,     // LED 모드, 커맨드
            LightDefault                = 0x23,     // LED 모드 3색

            RawMotion                   = 0x30,
            RawFlow,

            // 상태, 센서
            State                       = 0x40,     // 상태
            Attitude,                               // 자세
            Position,
            Altitude,                               // 고도
            Motion,                                 // Motion 센서 데이터(IMU)
            Range,                                   // 위치

            // 설정
            Count                       = 0x50,     // 카운트
            Bias,                                   // 엑셀, 자이로 바이어스 값
            Trim,                                   // 트림
            Weight,                                 // 무게
            LostConnection,                         // 연결이 끊긴 후 반응 시간 설정

            // Devices
            Motor                       = 0x60,     // 모터 제어 및 현재 제어값 확인
            MotorSingle,                            // 한 개의 모터 제어
            Buzzer,                                 // 부저 제어
            Vibrator,                               // 진동 제어

            // Input
            Button                      = 0x70,     // 버튼 입력
            Joystick,                               // 조이스틱 입력

            // Display
            DisplayClear                = 0x80,     // 화면 지우기
            DisplayInvert,                          // 화면 반전
            DisplayDrawPoint,                       // 점 그리기
            DisplayDrawLine,                        // 선 그리기
            DisplayDrawRect,                        // 사각형 그리기
            DisplayDrawCircle,                      // 원 그리기
            DisplayDrawString,                      // 문자열 쓰기
            DisplayDrawStringAlign,                 // 문자열 정렬하여 쓰기
            DisplayDrawImage,                       // 그림 그리기

            // Information Assembled
            InformationAssembledForController       = 0xA0,     // 자주 갱신되는 데이터 모음(조종기)
            InformationAssembledForEntry            = 0xA1,     // 자주 갱신되는 데이터 모음(드론)

            NavigationTarget                        = 0xD0,     // 네비게이션 목표점
            NavigationLocation                      = 0xD1,     // 네비게이션 가상 위치
            NavigationMonitor                       = 0xD2,
            NavigationHeading                       = 0xD3,
            NavigationCounter                       = 0xD4,

            GpsRtkNavigationState                   = 0xDA,     // RTK RAW 데이터 전송
            GpsRtkExtendedRawMeasurementData        = 0xDB,     // RTK RAW 데이터 전송

            EndOfType
        };
    }

    namespace CommandType
    {
        enum Type
        {
            None                        = 0x00,     // 이벤트 없음

            Stop                        = 0x01,     // 정지

            ModeControlFlight           = 0x02,     // 비행 제어 모드 설정
            Headless                    = 0x03,     // 헤드리스 모드 설정
            Trim                        = 0x04,     // 트림 변경

            ClearBias                   = 0x05,     // 자이로/엑셀 바이어스 리셋(트림도 같이 초기화 됨)
            ClearTrim                   = 0x06,     // 트림 초기화

            FlightEvent                 = 0x07,     // 비행 이벤트 실행

            SetDefault                  = 0x08,     // 기본 설정으로 초기화

            EndOfType
        };
    }

    namespace sendPacket
    {
        public struct packetTerm
        {
            public int control;
            public int state;
            public int attitude;
            public int altitude;
            public int motion;
            public int flow;
            public int range;
        };
    }

    namespace Command
    {
        public struct Command
        {
            public byte   commandType;   // 명령 타입
            public byte   option;        // 명령에 대한 옵션
        };

        public struct LightEvent
        {
            public Protocol.Command.Command  command;
            public Protocol.Light_Byrobot.Event  evnt;
        };

        public struct LightEventColor
        {
            public Protocol.Command.Command  command;
            public Protocol.Light_Byrobot.Event  evnt;
            public Light_Byrobot.Color  color;
        };

        public struct LightEventColors
        {
            public Protocol.Command.Command command;
            public Protocol.Light_Byrobot.Event evnt;
            public byte colors;
        };
    }

    // DeviceType ####################################
    namespace DeviceType
    {
        enum Type
        {
            None = 0,

            Drone       = 0x10,     // 드론(Server)

            Controller  = 0x20,     // 조종기(Client)

            Link        = 0x30,     // 링크 모듈(Client)
            LinkServer  = 0x31,     // 링크 모듈(Server, 링크 모듈이 서버로 동작하는 경우에만 통신 타입을 잠시 바꿈)

            ByScratch   = 0x80,     // 바이스크래치
            Scratch     = 0x81,     // 스크래치
            Entry       = 0x82,     // 네이버 엔트리

            Tester      = 0xA0,     // 테스터
            Monitor     = 0xA1,     // 모니터
            Updater     = 0xA2,     // 펌웨어 업데이트 도구
            Encrypter   = 0xA3,     // 암호화 도구

            EndOfType,

            Broadcasting = 0xFF
        };
    }

    // Light_Byrobot ####################################
    namespace Light_Byrobot
    {
        public struct Color
        {
            public byte r;           // Red
            public byte g;           // Green
            public byte b;           // Blue
        };

        public struct Manual
        {
            public ushort  flags;         // Flags 열거형을 조합한 값
            public byte  brightness;    // 밝기
        };

        public struct Mode
        {
            public byte  mode;       // LED 모드
            public ushort  interval;   // LED 모드의 갱신 주기
        };

        public struct ModeColor
        {
            public Protocol.Light_Byrobot.Mode   mode;
            Light_Byrobot.Color  color;
        };

        public struct ModeColors
        {
            public Protocol.Light_Byrobot.Mode  mode;
            public byte  colors;
        };

        public struct Event
        {
            public byte  evnt;      // LED 이벤트
            public ushort  interval;   // LED 이벤트 갱신 주기
            public byte  repeat;     // LED 이벤트 반복 횟수
        };

        public struct EventColor
        {
            public Protocol.Light_Byrobot.Event  evnt;
            public Light_Byrobot.Color color;
        };

        public struct EventColors
        {
            public Protocol.Light_Byrobot.Event  evnt;
            public byte  colors;
        };


    }

    // Display ####################################
    namespace Display
    {
        public struct ClearAll
        {
            public byte  pixel;
        };

        public struct Clear
        {
            public short  x;
            public short  y;
            public short  width;
            public short  height;
            public byte   pixel;
        };

        public struct Invert
        {
            public short  x;
            public short  y;
            public short  width;
            public short  height;
        };

        public struct DrawPoint
        {
            public short  x;
            public short  y;
            public byte   pixel;
        };

        public struct DrawLine
        {
            public short  x1;
            public short  y1;
            public short  x2;
            public short  y2;
            public byte   pixel;
            public byte   line;
        };

        public struct DrawRect
        {
            public short   x;
            public short   y;
            public short   width;
            public short   height;
            public byte    pixel;
            public byte    flagFill;
            public byte    line;
        };

        public struct DrawCircle
        {
            public short   x;
            public short   y;
            public short   radius;
            public byte    pixel;
            public byte    flagFill;
        };

        public struct DrawString
        {
            public short   x;
            public short   y;
            public byte    font;
            public byte    pixel;
        };

        public struct DrawStringAlign
        {
            public short   xStart;
            public short   xEnd;
            public short   y;
            public byte    align;
            public byte    font;
            public byte    pixel;
        };

        public struct DrawImage
        {
            public short   x;
            public short   y;
            public short   width;
            public short   height;
        };
    }

    public struct Header
    {
        public byte   dataType;       // 데이터의 형식(sbyte : s8)
        public byte   length;         // 데이터의 길이
        public byte   from;           // 데이터를 전송하는 장치의 DeviceType
        public byte   to;             // 데이터를 수신하는 장치의 DeviceType
    };

    public struct Ping
    {
        public ulong   systemTime;   // Ping을 전송하는 장치의 시각(ulong: u64)
    };

    public struct Ack
    {
        public ulong   systemTime;     // 수신 받은 시간
        public byte   dataType;       // 수신 받은 데이터 타입
        public ushort  crc16;          // 수신 받은 데이터의 crc16(ushort : u16)
    };

    public struct Error
    {
        public ulong   systemTime;             // 에러 메세지 송신 시각
        public uint    errorFlagsForSensor;    // 센서 오류 플래그
        public uint    errorFlagsForState;     // 상태 오류 플래그 (uint:u32)
    };

    public struct Request
    {
        public byte   dataType;          // 요청할 데이터 타입
    };

    public struct Information
    {
        public byte   modeUpdate;     // 현재 업데이트 모드

        public uint    modelNumber;    // 모델 번호
        public int    version_build;   // 현재 펌웨어의 빌드 버젼
        public int    version_minor;   // 현재 펌웨어의 마이너 버젼
        public int    version_major;   // 현재 펌웨어의 메이져 버젼

        public ushort  year;           // 빌드 년
        public byte   month;          // 빌드 월
        public byte   day;            // 빌드 일
    };

    public struct Version
    {
        public ushort  build;
        public byte   minor;
        public byte   major;
        public ulong   v;
    };

    public struct Address
    {
        public byte[] address;

        public Address(int init)
        {
            address = new byte[16];
        }
    };

    public struct State
    {
        public byte  modeSystem;         // 시스템 모드
        public byte  modeFlight;         // 비행 모드

        public byte  modeControlFlight;  // 비행 제어 모드
        public byte  modeMovement;       // 이동 상태
        public byte  headless;           // 헤드리스 모드
        public byte  controlSpeed;       // 조종 속도
        public byte  sensorOrientation;  // 센서 방향
        public byte  battery;            // 배터리량(0 ~ 10%)
    };

    public struct Attitude
    {
        public short  roll;         // Roll
        public short  pitch;        // Pitch
        public short  yaw;          // Yaw
    };

    public struct Position
    {
        public float    x;              // meter
        public float    y;              // meter
        public float    z;              // meter
    };

    public struct Altitude
    {
        public float  temperature;   //float : f32
        public float  pressure;
        public float  altitude;
        public float  rangeHeight;
    };

    public struct Bias
    {
        public short   accelX;         // X
        public short   accelY;         // Y
        public short   accelZ;         // Z

        public short   gyroRoll;       // Roll
        public short   gyroPitch;      // Pitch
        public short   gyroYaw;        // Yaw
    };

    public struct Trim
    {
        public short  roll;         // Roll
        public short  pitch;        // Pitch
        public short  yaw;          // Yaw
        public short  throttle;     // Throttle
    };

    public struct Weight
    {
        public float weight;         // Weight
    };

    public struct LostConnection
    {
        public ushort   timeNeutral;        // 조종 중립
        public ushort   timeLanding;        // 착륙
        public uint     timeStop;           // 정지
    };

    public struct Count
    {
        public uint  timeSystem;             // 시스템 동작 시간
        public uint  timeFlight;             // 비행 시간

        public ushort  countTakeOff;           // 이륙 횟수
        public ushort  countLanding;           // 착륙 횟수
        public ushort  countAccident;          // 충돌 횟수
    };

    public struct Motion
    {
        public short  accX;
        public short  accY;
        public short  accZ;

        public short  gyroRoll;
        public short  gyroPitch;
        public short  gyroYaw;

        public short  angleRoll;
        public short  anglePitch;
        public short  angleYaw;
    };

    public struct Range
    {
        public short  left;
        public short  front;
        public short  right;
        public short  rear;
        public short  top;
        public short  bottom;
    };

    public struct Flow
    {
        public float  x;
        public float  y;
    };

    public struct Motor
    {
        public byte  rotation;
        public short  value;
    };

    public struct MotorSingle
    {
        public byte target;
        public byte rotation;
        public short value;
    };

    public struct Button
    {
        public ushort  button;
        public byte  evnt;
    };

    public struct JoystickBlock
    {
        public  sbyte  x;
        public  sbyte  y;
        public  byte   direction;
        public  byte   evnt;
    };

    public struct Joystick
    {
        public Protocol.JoystickBlock  left;
        public Protocol.JoystickBlock  right;
    };

    public struct Buzzer
    {
        public byte    mode;       // 버저 작동 모드
        public ushort  value;    // Scale 또는 hz
        public ushort  time;   // 연주 시간(ms)
    };

    public struct Vibrator
    {
        public byte    mode;   // 모드(0은 set, 1은 reserve)
        public ushort  on;     // 진동을 켠 시간(ms)
        public ushort  off;    // 진동을 끈 시간(ms)
        public ushort  total;  // 전체 진행 시간(ms)
    };

    public struct Pairing
    {
        public ushort   address0;
        public ushort   address1;
        public ushort   address2;
        public byte     scramble;
        public byte     channel;
    };

    public struct Rssi
    {
        public sbyte  rssi;
    };

    public struct RawMotion
    {
        public short    accX;
        public short    accY;
        public short    accZ;

        public short    gyroRoll;
        public short    gyroPitch;
        public short    gyroYaw;
    };

    public struct RawFlow
    {
        public float    x;
        public float    y;
    };

    public struct InformationAssembledForController
    {
        public sbyte  angleRoll;              // 자세 Roll
        public sbyte  anglePitch;             // 자세 Pitch
        public short  angleYaw;               // 자세 Yaw

        public ushort  rpm;                    // RPM

        public short  positionX;              // meter x 10
        public short  positionY;              // meter x 10
        public short  positionZ;              // meter x 10

        public sbyte  speedX;                 // meter x 10
        public sbyte  speedY;                 // meter x 10

        public byte   rangeHeight;            // meter x 10

        public sbyte  rssi;                   // RSSI
    };

    public struct InformationAssembledForEntry
    {
        public short   angleRoll;
        public short   anglePitch;
        public short   angleYaw;

        public float   pressureTemperature;
        public float   pressureAltitude;

        public float   positionX;
        public float   positionY;

        public float   rangeHeight;
    };
}// Protocol #########################################################################################################################################


// Light_Byrobot #########################################################################################################################################
namespace Light_Byrobot
{
    namespace Drone
    {
        namespace Mode
        {
            enum Type
            {
                None,

                RearNone = 0x10,
                RearManual,             // 수동 제어
                RearHold,               // 지정한 색상을 계속 켬
                RearFlicker,            // 깜빡임
                RearFlickerDouble,      // 깜빡임(두 번 깜빡이고 깜빡인 시간만큼 꺼짐)
                RearDimming,            // 밝기 제어하여 천천히 깜빡임

                BodyNone = 0x20,
                BodyManual,             // 수동 제어
                BodyHold,               // 지정한 색상을 계속 켬
                BodyFlicker,            // 깜빡임
                BodyFlickerDouble,      // 깜빡임(두 번 깜빡이고 깜빡인 시간만큼 꺼짐)
                BodyDimming,            // 밝기 제어하여 천천히 깜빡임

                ANone = 0x30,
                AManual,                // 수동 제어
                AHold,                  // 지정한 색상을 계속 켬
                AFlicker,               // 깜빡임
                AFlickerDouble,         // 깜빡임(두 번 깜빡이고 깜빡인 시간만큼 꺼짐)
                ADimming,               // 밝기 제어하여 천천히 깜빡임

                BNone = 0x40,
                BManual,                // 수동 제어
                BHold,                  // 지정한 색상을 계속 켬
                BFlicker,               // 깜빡임
                BFlickerDouble,         // 깜빡임(두 번 깜빡이고 깜빡인 시간만큼 꺼짐)
                BDimming,               // 밝기 제어하여 천천히 깜빡임

                CNone = 0x50,
                CManual,                // 수동 제어
                CHold,                  // 지정한 색상을 계속 켬
                CFlicker,               // 깜빡임
                CFlickerDouble,         // 깜빡임(두 번 깜빡이고 깜빡인 시간만큼 꺼짐)
                CDimming,               // 밝기 제어하여 천천히 깜빡임

                EndOfType
            };
        }

        namespace Flags
        {
            enum Type
            {
                None        = 0x0000,

                Rear        = 0x0001,
                BodyRed     = 0x0002,
                BodyGreen   = 0x0004,
                BodyBlue    = 0x0008,

                A           = 0x0010,
                B           = 0x0020,
                CRed        = 0x0040,
                CGreen      = 0x0080,
                CBlue       = 0x0100,
            };
        }
    }

    namespace Controller
    {
        namespace Mode
        {
            enum Type
            {
                None,

                // Body
                BodyNone = 0x10,
                BodyManual,         // 수동 제어
                BodyHold,           // 지정한 색상을 계속 켬
                BodyFlicker,        // 깜빡임
                BodyFlickerDouble,  // 깜빡임(두 번 깜빡이고 깜빡인 시간만큼 꺼짐)
                BodyDimming,        // 밝기 제어하여 천천히 깜빡임

                EndOfType
            };
        }

        namespace Flags
        {
            enum Type
            {
                None        = 0x00,

                BodyRed     = 0x01,
                BodyGreen   = 0x02,
                BodyBlue    = 0x04,
            };
        }
    }

    namespace Colors
    {
        enum Type
        {
            AliceBlue,              // 0x00
            AntiqueWhite,           // 0x01
            Aqua,                   // 0x02
            Aquamarine,             // 0x03
            Azure,                  // 0x04
            Beige,                  // 0x05
            Bisque,                 // 0x06
            Black,                  // 0x07
            BlanchedAlmond,         // 0x08
            Blue,                   // 0x09
            BlueViolet,             // 0x0A
            Brown,                  // 0x0B
            BurlyWood,              // 0x0C
            CadetBlue,              // 0x0D
            Chartreuse,             // 0x0E
            Chocolate,              // 0x0F
            Coral,                  // 0x10
            CornflowerBlue,         // 0x11
            Cornsilk,               // 0x12
            Crimson,                // 0x13
            Cyan,                   // 0x14
            DarkBlue,               // 0x15
            DarkCyan,               // 0x16
            DarkGoldenRod,          // 0x17
            DarkGray,               // 0x18
            DarkGreen,              // 0x19
            DarkKhaki,              // 0x1A
            DarkMagenta,            // 0x1B
            DarkOliveGreen,         // 0x1C
            DarkOrange,             // 0x1D
            DarkOrchid,             // 0x1E
            DarkRed,                // 0x1F
            DarkSalmon,             // 0x20
            DarkSeaGreen,           // 0x21
            DarkSlateBlue,          // 0x22
            DarkSlateGray,          // 0x23
            DarkTurquoise,          // 0x24
            DarkViolet,             // 0x25
            DeepPink,               // 0x26
            DeepSkyBlue,            // 0x27
            DimGray,                // 0x28
            DodgerBlue,             // 0x29
            FireBrick,              // 0x2A
            FloralWhite,            // 0x2B
            ForestGreen,            // 0x2C
            Fuchsia,                // 0x2D
            Gainsboro,              // 0x2E
            GhostWhite,             // 0x2F
            Gold,                   // 0x30
            GoldenRod,              // 0x31
            Gray,                   // 0x32
            Green,                  // 0x33
            GreenYellow,            // 0x34
            HoneyDew,               // 0x35
            HotPink,                // 0x36
            IndianRed,              // 0x37
            Indigo,                 // 0x38
            Ivory,                  // 0x39
            Khaki,                  // 0x3A
            Lavender,               // 0x3B
            LavenderBlush,          // 0x3C
            LawnGreen,              // 0x3D
            LemonChiffon,           // 0x3E
            LightBlue,              // 0x3F
            LightCoral,             // 0x40
            LightCyan,              // 0x41
            LightGoldenRodYellow,   // 0x42
            LightGray,              // 0x43
            LightGreen,             // 0x44
            LightPink,              // 0x45
            LightSalmon,            // 0x46
            LightSeaGreen,          // 0x47
            LightSkyBlue,           // 0x48
            LightSlateGray,         // 0x49
            LightSteelBlue,         // 0x4A
            LightYellow,            // 0x4B
            Lime,                   // 0x4C
            LimeGreen,              // 0x4D
            Linen,                  // 0x4E
            Magenta,                // 0x4F
            Maroon,                 // 0x50
            MediumAquaMarine,       // 0x51
            MediumBlue,             // 0x52
            MediumOrchid,           // 0x53
            MediumPurple,           // 0x54
            MediumSeaGreen,         // 0x55
            MediumSlateBlue,        // 0x56
            MediumSpringGreen,      // 0x57
            MediumTurquoise,        // 0x58
            MediumVioletRed,        // 0x59
            MidnightBlue,           // 0x5A
            MintCream,              // 0x5B
            MistyRose,              // 0x5C
            Moccasin,               // 0x5D
            NavajoWhite,            // 0x5E
            Navy,                   // 0x5F
            OldLace,                // 0x60
            Olive,                  // 0x61
            OliveDrab,              // 0x62
            Orange,                 // 0x63
            OrangeRed,              // 0x64
            Orchid,                 // 0x65
            PaleGoldenRod,          // 0x66
            PaleGreen,              // 0x67
            PaleTurquoise,          // 0x68
            PaleVioletRed,          // 0x69
            PapayaWhip,             // 0x6A
            PeachPuff,              // 0x6B
            Peru,                   // 0x6C
            Pink,                   // 0x6D
            Plum,                   // 0x6E
            PowderBlue,             // 0x6F
            Purple,                 // 0x70
            RebeccaPurple,          // 0x71
            Red,                    // 0x72
            RosyBrown,              // 0x73
            RoyalBlue,              // 0x74
            SaddleBrown,            // 0x75
            Salmon,                 // 0x76
            SandyBrown,             // 0x77
            SeaGreen,               // 0x78
            SeaShell,               // 0x79
            Sienna,                 // 0x7A
            Silver,                 // 0x7B
            SkyBlue,                // 0x7C
            SlateBlue,              // 0x7D
            SlateGray,              // 0x7E
            Snow,                   // 0x7F
            SpringGreen,            // 0x80
            SteelBlue,              // 0x81
            Tan,                    // 0x82
            Teal,                   // 0x83
            Thistle,                // 0x84
            Tomato,                 // 0x85
            Turquoise,              // 0x86
            Violet,                 // 0x87
            Wheat,                  // 0x88
            White,                  // 0x89
            WhiteSmoke,             // 0x8A
            Yellow,                 // 0x8B
            YellowGreen,            // 0x8C

            EndOfType
        };
    }

    public struct Color
    {
        public byte r;           // Red
        public byte g;           // Green
        public byte b;           // Blue
    };

}// Light_Byrobot #########################################################################################################################################


// Display #########################################################################################################################################
namespace Display
{
    namespace Pixel
    {
        enum Type
        {
            Black,
            White,
            Inverse
        };
    }

    namespace Font
    {
        enum Type
        {
            LiberationMono5x8,
            LiberationMono10x16,
        };
    }

    namespace Align
    {
        enum Type
        {
            Left,
            Center,
            Right
        };
    }

    namespace Line
    {
        enum Type
        {
            Solid,
            Dotted,
            Dashed,
        };
    }
}// Display #########################################################################################################################################


// Control #########################################################################################################################################
namespace Control_2
{
    public struct Quad8
    {
        public sbyte   roll;       // roll
        public sbyte   pitch;      // pitch
        public sbyte   yaw;        // yaw
        public sbyte   throttle;   // throttle
    };

    public struct Quad8AndRequestData
    {
        public sbyte   roll;       // roll
        public sbyte   pitch;      // pitch
        public sbyte   yaw;        // yaw
        public sbyte   throttle;   // throttle

        public byte   dataType;   // DataType
    };

    public struct Position16
    {
        public short   positionX;          // meter    x 10
        public short   positionY;          // meter    x 10
        public short   positionZ;          // meter    x 10
        public short   velocity;           // m/s      x 10


        public short   heading;            // degree
        public short   rotationalVelocity; // deg/s
    };

    public struct Position
    {
        public float   positionX;              // meter
        public float   positionY;              // meter
        public float   positionZ;              // meter

        public float    velocityX;              // m/s
        public float    velocityY;              // m/s
        public float    velocityZ;              // m/s

        public float    heading;                // degree
        public float    rotationalVelocity;     // deg/s
    };
}// Control #########################################################################################################################################


// ModelNumber #########################################################################################################################################
namespace ModelNumber
{
    enum Type
    {
        //                          AAAABBCC, AAAA(Project Number), BB(Device Type), CC(Revision)
        Drone_4_Drone_P4        = 0x00041004,       // Drone_4_Drone_P4

        Drone_4_Controller_P1   = 0x00042001,       // Drone_4_Controller_P1
        Drone_4_Controller_P2   = 0x00042002,       // Drone_4_Controller_P2

        Drone_4_Link_P0         = 0x00043000,       // Drone_4_Link_P0

        Drone_4_Tester_P2       = 0x0004A002,       // Drone_4_Tester_P2
        Drone_4_Monitor_P2      = 0x0004A102,       // Drone_4_Monitor_P2
    };
}// ModelNumber #########################################################################################################################################


// ErrorFlagsForSensor #########################################################################################################################################
namespace ErrorFlagsForSensor
{
    enum Type
    {
        None                        = 0x00000000,

        Motion_NoAnswer             = 0x00000001,   // Motion 센서 응답 없음
        Motion_WrongValue           = 0x00000002,
        Motion_NotCalibrated        = 0x00000004,   // Gyro Bias 보정이 완료되지 않음
        Motion_Calibrating          = 0x00000008,   // Gyro Bias 보정 중

        Pressure_NoAnswer           = 0x00000010,   // 압력센서 응답 없음
        Pressure_WrongValue         = 0x00000020,

        RangeGround_NoAnswer        = 0x00000100,   // 바닥 거리센서 응답 없음
        RangeGround_WrongValue      = 0x00000200,

        Flow_NoAnswer               = 0x00001000,   // Flow 센서 응답 없음
        Flow_WrongValue             = 0x00002000,

        Battery_NoAnswer            = 0x00010000,   // 배터리 응답 없음
        Battery_WrongValue          = 0x00020000,
        Battery_NotCalibrated       = 0x00040000,   // 배터리 입력값 보정이 완료되지 않음
    };
}// ErrorFlagsForSensor #########################################################################################################################################


// ErrorFlagsForState #########################################################################################################################################
namespace ErrorFlagsForState
{
    enum Type
    {
        None                                    = 0x00000000,

        NotRegistered                           = 0x00000001,   // 장치 등록이 안됨
        FlashReadLock_UnLocked                  = 0x00000002,   // 플래시 메모리 읽기 Lock이 안 걸림
        BootloaderWriteLock_UnLocked            = 0x00000004,   // 부트로더 영역 쓰기 Lock이 안 걸림

        TakeoffFailure_CheckPropellerAndMotor   = 0x00000010,   // 이륙 실패
        CheckPropellerVibration                 = 0x00000020,   // 프로펠러 진동발생
    };
}// ErrorFlagsForState #########################################################################################################################################


// Mode #########################################################################################################################################
namespace Mode
{
    namespace Control
    {
        namespace Flight
        {
            enum Type
            {
                None = 0,

                Attitude    = 0x10, // 자세 - X,Y는 각도(deg)로 입력받음, Z,Yaw는 속도(m/s)로 입력 받음
                Position    = 0x11, // 위치 - X,Y,Z,Yaw는 속도(m/s)로 입력 받음
                Function    = 0x12, // 기능 - X,Y,Z,Yaw는 속도(m/s)로 입력 받음

                EndOfType
            };
        }
    }

    namespace System
    {
        enum Type
        {
            None = 0,

            Boot,               // 부팅
            Start,              // 시작 코드 실행
            Running,            // 메인 코드 동작
            ReadyToReset,       // 리셋 대기(1초 뒤 리셋)
            Error,              // 오류

            EndOfType
        };
    }

    namespace Flight
    {
        enum Type
        {
            None = 0,

            Ready = 0x10,       // 준비

            Start,              // 이륙 준비
            TakeOff,            // 이륙 (Flight로 자동전환)
            Flight,             // 비행
            Landing,            // 착륙
            Flip,               // 회전
            Reverse,            // 뒤집기

            Stop = 0x20,        // 강제 정지

            Accident = 0x30,    // 사고 (Ready로 자동전환)
            Error,              // 오류

            Test = 0x40,        // 테스트 모드

            EndOfType
        };
    }

    namespace Update
    {
        enum Type
        {
            None,

            Ready,              // 업데이트 가능 상태
            Update,             // 업데이트 중
            Complete,           // 업데이트 완료

            Faild,              // 업데이트 실패(업데이트 완료까지 갔으나 body의 CRC16이 일치하지 않는 경우 등)

            NotAvailable,       // 업데이트 불가능 상태(Debug 모드 등)
            RunApplication,     // 어플리케이션 동작 중
            NotRegistered,      // 등록되지 않은 장치

            EndOfType
        };
    }

    namespace Movement
    {
        enum Type
        {
            None        = 0x00,
            Hovering    = 0x01,     // 호버링
            Moving      = 0x02      // 이동 중
        };
    }
}// Mode #########################################################################################################################################


// SensorOrientation #########################################################################################################################################
namespace SensorOrientation_2
{
    enum Type
    {
        None = 0,

        Normal,             // 정상
        ReverseStart,       // 뒤집히기 시작
        Reversed,           // 뒤집힘

        EndOfType
    };
}// SensorOrientation #########################################################################################################################################


// Direction_Byrobot #########################################################################################################################################
namespace Direction_Byrobot
{
    enum Type
    {
        None = 0,

        Left,
        Front,
        Right,
        Rear,

        Top,
        Bottom,

        Center,

        EndOfType
    };
}// Direction_Byrobot #########################################################################################################################################


// Headless #########################################################################################################################################
namespace Headless
{
    enum Type
    {
        None = 0,

        Headless,   // 사용자 중심 좌표
        Normal,     // 드론 중심 좌표

        EndOfType
    };
}// Headless #########################################################################################################################################


// Trim #########################################################################################################################################
namespace Trim_2
{
    enum Type
    {
        None = 0,

        RollIncrease,       // Roll 증가
        RollDecrease,       // Roll 감소
        PitchIncrease,      // Pitch 증가
        PitchDecrease,      // Pitch 감소
        YawIncrease,        // Yaw 증가
        YawDecrease,        // Yaw 감소
        ThrottleIncrease,   // Throttle 증가
        ThrottleDecrease,   // Throttle 감소

        Reset,              // 전체 트림 리셋

        EndOfType
    };
}// Trim #########################################################################################################################################


// Rotation #########################################################################################################################################
namespace Rotation
{
    enum Type
    {
        None = 0,

        Clockwise,              // 시계 방향
        Counterclockwise,       // 반시계 방향

        EndOfType
    };
}// Rotation #########################################################################################################################################


// Rotation #########################################################################################################################################
namespace Rotation
{
    namespace Part
    {
        enum Type
        {
            M1,     // Front Left
            M2,     // Front Right
            M3,     // Rear Right
            M4,     // Rear Left

            EndOfPart,

            All
        };
    }
}// Rotation #########################################################################################################################################


// FlightEvent #########################################################################################################################################
namespace FlightEvent_2
{
    enum Type
    {
        None = 0,               // 없음

        Stop = 0x10,            // 정지
        TakeOff,                // 이륙
        Landing,                // 착륙

        Reverse,                // 뒤집기

        FlipFront,              // 회전
        FlipRear,               // 회전
        FlipLeft,               // 회전
        FlipRight,              // 회전

        Return,                 // 시작 위치로 돌아가기

        ResetHeading = 0xA0,    // 헤딩 리셋(앱솔루트 모드 일 때 현재 heading을 0도로 변경)

        EndOfType
    };
}// FlightEvent #########################################################################################################################################


// Joystick #########################################################################################################################################
namespace Joystick
{
    // 조이스틱 방향
    namespace Direction
    {
        enum Type
        {
            None    = 0,        // 정의하지 않은 영역(무시함)

            VT      = 0x10,     //   위(세로)
            VM      = 0x20,     // 중앙(세로)
            VB      = 0x40,     // 아래(세로)

            HL      = 0x01,     //   왼쪽(가로)
            HM      = 0x02,     //   중앙(가로)
            HR      = 0x04,     // 오른쪽(가로)

            TL = 0x11,  TM = 0x12,  TR = 0x14,
            ML = 0x21,  CN = 0x22,  MR = 0x24,
            BL = 0x41,  BM = 0x42,  BR = 0x44
        };
    }

    namespace Event
    {
        enum Type
        {
            None    = 0,        // 이벤트 없음

            In,                 // 특정 영역에 진입
            Stay,               // 특정 영역에서 상태 유지
            Out,                // 특정 영역에서 벗어남

            EndOfType
        };
    }
}// Joystick #########################################################################################################################################

namespace Button_2
{
    namespace Drone
    {
        namespace ButtonType
        {
            enum Type
            {
                None        = 0x0000,

                // 버튼
                Reset       = 0x0001
            };
        }
    }

    namespace Controller
    {
        namespace ButtonType
        {
            enum Type
            {
                None                = 0x0000,

                // 버튼
                FrontLeftTop        = 0x0001,
                FrontLeftBottom     = 0x0002,
                FrontRightTop       = 0x0004,
                FrontRightBottom    = 0x0008,

                TopLeft             = 0x0010,
                TopRight            = 0x0020,   // POWER ON/OFF

                MidUp               = 0x0040,
                MidLeft             = 0x0080,
                MidRight            = 0x0100,
                Miy             = 0x0200,

                BottomLeft          = 0x0400,
                BottomRight         = 0x0800,
            };
        }
    }

    namespace Event
    {
        enum Type
        {
            None,

        y,               // 누르기 시작
            Press,              // 누르는 중
            Up,                 // 뗌

            EndContinuePress    // 연속 입력 종료
        };
    }
}


// Buzzer #########################################################################################################################################
namespace Buzzer
{
    namespace Mode
    {
        enum Type
        {
            Stop                = 0,    // 정지(Mode에서의 Stop은 통신에서 받았을 때 Buzzer를 끄는 용도로 사용, set으로만 호출)

            MuteInstantally     = 1,    // 묵음 즉시 적용
            MuteContinually     = 2,    // 묵음 예약

            ScaleInstantally    = 3,    // 음계 즉시 적용
            ScaleContinually    = 4,    // 음계 예약

            HzInstantally       = 5,    // 주파수 즉시 적용
            HzContinually       = 6,    // 주파수 예약

            EndOfType
        };
    }

    namespace Scale
    {
        enum Type
        {
            C1, CS1, D1, DS1, E1, F1, FS1, G1, GS1, A1, AS1, B1,
            C2, CS2, D2, DS2, E2, F2, FS2, G2, GS2, A2, AS2, B2,
            C3, CS3, D3, DS3, E3, F3, FS3, G3, GS3, A3, AS3, B3,
            C4, CS4, D4, DS4, E4, F4, FS4, G4, GS4, A4, AS4, B4,

            C5, CS5, D5, DS5, E5, F5, FS5, G5, GS5, A5, AS5, B5,
            C6, CS6, D6, DS6, E6, F6, FS6, G6, GS6, A6, AS6, B6,
            C7, CS7, D7, DS7, E7, F7, FS7, G7, GS7, A7, AS7, B7,
            C8, CS8, D8, DS8, E8, F8, FS8, G8, GS8, A8, AS8, B8,

            EndOfType,

            Mute    = 0xEE,     // 묵음
            Fin     = 0xFF      // 악보의 끝
        };
    }
}// Buzzer #########################################################################################################################################


// Vibrator #########################################################################################################################################
namespace Vibrator
{
    namespace Mode
    {
        enum Type
        {
            Stop            = 0,    // 정지

            Instantally     = 1,    // 즉시 적용
            Continually     = 2,    // 예약

            EndOfType
        };
    }
}// Vibrator #########################################################################################################################################






// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%






// RobotConnector =============================================================================================================================================================
public class RobotConnector2 : MonoBehaviour
{
    

    public int baudrate = 57600;
    private SerialPort _serialPort;
    public List<string> portNames = new List<string>(); // 탐색한 포트들
    public string portName; // 연결할 포트명

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


    public int  takeoffPressed = 0;
    public int  landingPressed = 0;
    public int  stopPressed = 0;
    public int  trimPressed = 0;
    public int  clearbiasPressed = 0;
    public int  SpeedPressed = 0;
    public int  _nSpeed = 1;

//    public int  mode_linkPressed = 0;
//    public int  mode_controlPressed = 0;

    public int  _nSendCount_VerRC = 0; // 조종기 버전 요청 연속 전송횟수
    public int  _nSendCount_VerRFModule = 0; // RF모듈 버전 요청 연속 전송횟수
    public int  _nSendCount_VerDrone = 0; // 드론 버전 요청 연속 전송횟수



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

    public byte  lightRed = 0;
    public byte  lightGreen = 0;
    public byte  lightBlue = 0;
    public int   lightSend = 0;

    public int Flip_Front = 0; // 플립 앞
    public int Flip_Back = 0; // 플립 뒤
    public int Flip_Left = 0; // 플립 왼쪽
    public int Flip_Right = 0; // 플립 오른쪽

    public int Count = 0; // 비행 관련 카운트 값 (이착륙 충돌 횟수)



    public bool  _opened = false; // 포트 열림
    public bool  _connected = false; // 실제 드론 접속
    private int _sendCounter = 0;
    //private ulong _gCounter = 0;

    float _fSendInterval = 0.01f; // packetSendingHandler() 함수 인보크 재실행 간격



    // 각 센서 정밀값 활성 (RobotConnector2용)
    public bool _bIsDetailImageFlow = false;
    public bool _bIsDetailRange = false;
    public bool _bIsDetailAxis = false;
    public bool _bIsDetailSensor = false;

    public bool _bIsAllSensor = true; // 모든 센서 균일하게 작동





    // // Awake ----------------------------------------------------------------------------------------------
    // void Awake()
    // {
    //     _serialPort = new SerialPort();
    //     _serialPort.DtrEnable = false;
    //     _serialPort.RtsEnable = false;
    //     _serialPort.DataBits = 8;
    //     _serialPort.ReadTimeout = 1;
    //     _serialPort.WriteTimeout = 1000;
    //     _serialPort.Parity = Parity.None;
    //     _serialPort.StopBits = StopBits.One;
    //     PortSearch();
    //     Connect();
    //     ResetData();

    //     Invoke("packetSendingHandler", 0.05f);
    // }


    public void Connect_Manager()
    {
        _serialPort = new SerialPort();
        _serialPort.DtrEnable = false;
        _serialPort.RtsEnable = false;
        _serialPort.DataBits = 8;
        _serialPort.ReadTimeout = 1;
        _serialPort.WriteTimeout = 1000;
        _serialPort.Parity = Parity.None;
        _serialPort.StopBits = StopBits.One;
        PortSearch();
        Connect();
        ResetData();

        Invoke("packetSendingHandler", 0.05f);
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
            //Debug.Log(tempBytes[0] + "  |  " + tempBytes[1] + "  |  " + tempBytes[2] + "  |  " + tempBytes[3] + "  |  " + tempBytes[4]);

            // if (tempBytes != null)
            // {
                            //    for (int i = 0; i < tempBytes.Length; i++)
                            //    {
                            //        Debug.Log("[" + i + "] " + Convert.ToString(tempBytes[i], 16));
                            //    }
                
                
                            //    Debug.Log(" [0]: " + Convert.ToString(tempBytes[0], 16) +
                            //        " [1]: " + Convert.ToString(tempBytes[1], 16) +
                            //        " [2]: " + Convert.ToString(tempBytes[2], 16) +
                            //        " [3]: " + Convert.ToString(tempBytes[3], 16) +
                            //        " [4]: " + Convert.ToString(tempBytes[4], 16) +
                            //        " [5]: " + Convert.ToString(tempBytes[5], 16));
            //}
    }


    // packetSendingHandler ----------------------------------------------------------------------------------------------------------
    private void packetSendingHandler()
    {
        if (_opened == true)
        {
            _sendCounter++;
            //if (_sendCounter > 12)
                _sendCounter %= 12;


            if (_sendCounter == 0) // state -------------------------------------------------------------------
            {
                //Debug.Log("state");
                try
                {
                    byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x10, 0x40, 0x5F, 0x8F };
                    _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                    //Debug.Log("---------- Type.state"); 
                    //term.state = 0;
                }
                catch (Exception)
                {
                    Console.WriteLine("exceptipn : failed to sending state Packet");
                }
            }
            // position ---------------------------------------------------------------------------------------------------------------
            else if (_sendCounter == 1)
            {
                //Debug.Log("position");
                try
                {
                    byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x10, 0x42, 0x1D, 0xAF };
                    _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                    //Debug.Log("---------- Type.position"); 
                }
                catch (Exception)
                {
                    Console.WriteLine("exceptipn : failed to sending position Packet");
                }
            }
//            else if (_sendCounter == 1) // attitude -----------------------------------------------------------
//            {
//                //Debug.Log("attitude");
//                try
//                {
//                    byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x10, 0x41, 0x7E, 0x9F };
//                    _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
//                    //Debug.Log("---------- Type.attitude");
//                    //term.attitude = 0;
//                }
//                catch (Exception)
//                {
//                    Console.WriteLine("exceptipn : failed to sending attitude Packet");
//                }
//            }
            // altitude ------------------------------------------------------------------------------------------------------------
            else if (  ((_bIsAllSensor) && (_sendCounter == 3)) ||
                       ((!_bIsAllSensor) && (_bIsDetailSensor) && ((_sendCounter == 3)||(_sendCounter == 5)||(_sendCounter == 7)||(_sendCounter == 9)))   )
            {
                //Debug.Log("altitude");
                try
                {
                    byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x10, 0x43, 0x3C, 0xBF };
                    _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                    //term.altitude = 0;
                    //Debug.Log("---------- Type.altitude");
                }
                catch (Exception)
                {
                    Console.WriteLine("exceptipn : failed to sending attitude Packet");
                }
            }
            // motion ------------------------------------------------------------------------------------------------------------
            else if (  ((_bIsAllSensor) && (_sendCounter == 5)) ||
                       ((!_bIsAllSensor) && (_bIsDetailAxis) && ((_sendCounter == 3)||(_sendCounter == 5)||(_sendCounter == 7)||(_sendCounter == 9)))   )
            {
                //Debug.Log("motion");
                try
                {
                    byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x10, 0x44, 0xDB, 0xCF };
                    _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                    //Debug.Log("---------- Type.motion"); 
                }
                catch (Exception)
                {
                    Console.WriteLine("exceptipn : failed to sending motion Packet");
                }
            }
            // flow --------------------------------------------------------------------------------------------------------------
            else if (  ((_bIsAllSensor) && (_sendCounter == 7)) ||
                       ((!_bIsAllSensor) && (_bIsDetailImageFlow) && ((_sendCounter == 3)||(_sendCounter == 5)||(_sendCounter == 7)||(_sendCounter == 9)))   )
            {
                //Debug.Log("flow");
                try
                {
                    byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x10, 0x31, 0xE9, 0xE1 };
                    _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                    //Debug.Log("---------- Type.flow"); 
                }
                catch (Exception)
                {
                    Console.WriteLine("exceptipn : failed to sending flow Packet");
                }
            }
            // range ---------------------------------------------------------------------------------------------------------------
            else if (  ((_bIsAllSensor) && (_sendCounter == 9)) ||
                       ((!_bIsAllSensor) && (_bIsDetailRange) && ((_sendCounter == 3)||(_sendCounter == 5)||(_sendCounter == 7)||(_sendCounter == 9)))   )
            {
                //Debug.Log("range");
                try
                {
                    byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x10, 0x45, 0xFA, 0xDF };
                    _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                    //Debug.Log("---------- Type.range"); 
                }
                catch (Exception)
                {
                    Console.WriteLine("exceptipn : failed to sending range Packet");
                }
            }
            // trim ---------------------------------------------------------------------------------------------------------------
            else if (_sendCounter == 11)
            {
                //Debug.Log("trim");
                try
                {
                    byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x10, 0x52, 0x2C, 0xBD };
                    _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                    //Debug.Log("---------- Type.trim"); 
                }
                catch (Exception)
                {
                    Console.WriteLine("exceptipn : failed to sending trim Packet");
                }
            }
            else // control 및 기타 잠시 보내는 신호들 -------------------------------------------------------------------------------------
            {
                // 조종기 버전 요청 연속 전송-----------------------------
                if (_nSendCount_VerRC > 0)
                {
                    //Debug.Log("VerRC");
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x20, 0x07, 0xE9, 0xB2 };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        //Debug.Log("---------- Type.VerRC"); 
                        _nSendCount_VerRC--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to VerRC Packet");
                    }
                }

                // RF모듈 버전 요청 연속 전송-----------------------------
                else if (_nSendCount_VerRFModule > 0)
                {
                    //Debug.Log("VerRFModule");
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x70, 0x30, 0x07, 0xC8, 0x52 };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        //Debug.Log("---------- Type.VerRFModule"); 
                        _nSendCount_VerRFModule--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to VerRFModule Packet");
                    }
                }

                // 드론 버전 요청 연속 전송-----------------------------
                else if (_nSendCount_VerDrone > 0)
                {
                    //Debug.Log("VerDrone");
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x10, 0x07, 0x7C, 0xB7 };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        //Debug.Log("---------- Type.VerDrone"); 
                        _nSendCount_VerDrone--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to VerDrone Packet");
                    }
                }
                else if (stopPressed > 0) // 강제정지신호 ---------------------------------------------------------------------------
                {
                    //Debug.Log("stop");
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x80, 0x10, 0x01, 0x00, 0xCD, 0xB6 };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        //Debug.Log("---------- Type.stop"); 
                        stopPressed--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to stop Packet");
                    }
                }
                else if (landingPressed > 0) // 착륙신호 ---------------------------------------------------------------------------
                {
                    //Debug.Log("landing");
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x80, 0x10, 0x07, 0x12, 0x18, 0x2E };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        // Debug.Log("---------- Type.landing"); 
                        landingPressed--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to landing Packet");
                    }
                }
                else if (takeoffPressed > 0) // 이륙신호 ---------------------------------------------------------------------------
                {
                    Debug.Log("takeoff  , " + _sendCounter);
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x80, 0x10, 0x07, 0x11, 0x7B, 0x1E };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        // Debug.Log("---------- Type.takeoff");
                        takeoffPressed--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to takeoff Packet");
                    }
                }
                else if (trimPressed > 0) // 트림신호 ---------------------------------------------------------------------------
                {
                    //Debug.Log("trim");
                    byte[] Roll = BitConverter.GetBytes(trimRoll);
                    byte[] Pitch = BitConverter.GetBytes(trimPitch);
                    byte[] Yaw = BitConverter.GetBytes(trimYaw);
                    byte[] Throttle = BitConverter.GetBytes(trimThrottle);

                    byte[] tempBuff2 = { 0x52, 0x08, 0x80, 0x10, Roll[0], Roll[1], Pitch[0], Pitch[1], Yaw[0], Yaw[1], Throttle[0], Throttle[1] };
                    byte crcL2, crcH2;
                    ushort crc2 = crc16_ccitt(tempBuff2, 0, tempBuff2.Length);    //0A 55 crc crc 는 제외함
                    crcL2 = (byte)(crc2 & 0xFF);
                    crcH2 = (byte)((crc2 & 0xFF00) >> 8);

                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x52, 0x08, 0x80, 0x10, Roll[0], Roll[1], Pitch[0], Pitch[1], Yaw[0], Yaw[1], Throttle[0], Throttle[1], crcL2, crcH2 };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        //Debug.Log("---------- Type.trim");
                        trimPressed--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to trim Packet");
                    }
                }
                else if (clearbiasPressed > 0) // 리셋 바이어스 신호 ---------------------------------------------------------------------------
                {
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x80, 0x10, 0x05, 0x00, 0x09, 0x7A };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        // Debug.Log("---------- clear bias");
                        clearbiasPressed--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to clearbias Packet");
                    }
                }
                else if (SpeedPressed > 0) // 스피드 변경 신호 --------------------------------------------------------------------------------
                {
                    byte[] tempBuff2 = { 0x11, 0x02, 0x80, 0x10, 0x04, (byte)_nSpeed };
                    byte crcL2, crcH2;
                    ushort crc2 = crc16_ccitt(tempBuff2, 0, tempBuff2.Length);    //0A 55 crc crc 는 제외함
                    crcL2 = (byte)(crc2 & 0xFF);
                    crcH2 = (byte)((crc2 & 0xFF00) >> 8);

                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x80, 0x10, 0x04, (byte)_nSpeed, crcL2, crcH2 };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        //Debug.Log("---------- Type.Speed");
                        SpeedPressed--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to Speed Packet");
                    }
                }
//                else if (mode_linkPressed > 0) // 모드 링크 신호 ---------------------------------------------------------------------------
//                {
//                    try
//                    {
//                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x70, 0x20, 0x0A, 0x80, 0x57, 0xA1, 0xFF };
//                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
//                        // Debug.Log("---------- mode link");
//                        mode_linkPressed--;
//                    }
//                    catch (Exception)
//                    {
//                        Console.WriteLine("exceptipn : failed to mode_link Packet");
//                    }
//                }
//                else if (mode_controlPressed > 0) // 모드 컨트롤 신호 ---------------------------------------------------------------------------
//                {
//                    try
//                    {
//                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x70, 0x20, 0x0A, 0x10, 0xEE, 0x22, 0xFF };
//                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
//                        // Debug.Log("---------- mode control");
//                        mode_controlPressed--;
//                    }
//                    catch (Exception)
//                    {
//                        Console.WriteLine("exceptipn : failed to mode_control Packet");
//                    }
//                }
                else if (lightSend > 0) // LED신호 ---------------------------------------------------------------------------
                {
                    //Debug.Log("light"); //0x21 -> 0x23
                    byte[] tempBuff2 = { 0x23, 0x06, 0x80, 0x10, 0x22, 0xFF, 0x00, lightRed, lightGreen, lightBlue };
                    byte crcL2, crcH2;
                    ushort crc2 = crc16_ccitt(tempBuff2, 0, tempBuff2.Length);    //0A 55 crc crc 는 제외함
                    crcL2 = (byte)(crc2 & 0xFF);
                    crcH2 = (byte)((crc2 & 0xFF00) >> 8);

                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x23, 0x06, 0x80, 0x10, 0x22, 0xFF, 0x00, lightRed, lightGreen, lightBlue, crcL2, crcH2 };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        //Debug.Log("---------- Type.Light");
                        lightSend--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to LED Packet");
                    }
                }
                else if (Flip_Front > 0) // 플립 앞 ----------------------------------------------------------------------------
                {
                    //Debug.Log("flip_front");
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x80, 0x10, 0x07, 0x14, 0xDE, 0x4E };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        // Debug.Log("---------- Type.Flip_Front"); 
                        Flip_Front--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to flip_front Packet");
                    }
                }
                else if (Flip_Back > 0) // 플립 뒤 ----------------------------------------------------------------------------
                {
                    //Debug.Log("flip_back");
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x80, 0x10, 0x07, 0x15, 0xFF, 0x5E };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        // Debug.Log("---------- Type.flip_back"); 
                        Flip_Back--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to flip_back Packet");
                    }
                }
                else if (Flip_Left > 0) // 플립 좌 ----------------------------------------------------------------------------
                {
                    //Debug.Log("flip_left");
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x80, 0x10, 0x07, 0x16, 0x9C, 0x6E };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        // Debug.Log("---------- Type.flip_left"); 
                        Flip_Left--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to flip_left Packet");
                    }
                }
                else if (Flip_Right > 0) // 플립 우 ----------------------------------------------------------------------------
                {
                    //Debug.Log("flip_right");
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x80, 0x10, 0x07, 0x17, 0xBD, 0x7E };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        // Debug.Log("---------- Type.flip_right"); 
                        Flip_Right--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to flip_right Packet");
                    }
                }
                else if (Count > 0) // 비행 관련 데이터 카운트 -------------------------------------------------------------------------
                {
                    //Debug.Log("Count");
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x10, 0x50, 0x6E, 0x9D };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        // Debug.Log("---------- Type.Count"); 
                        Count--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to Count Packet");
                    }
                }
                else // 실 조종신호 전송 !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                {
                    //Debug.Log("control");
                    byte[] tempBuff = { 0x10, 0x04, 0x80, 0x10, (byte)quad8.roll, (byte)quad8.pitch, (byte)quad8.yaw, (byte)quad8.throttle };
                    byte crcL, crcH;
                    ushort crc = crc16_ccitt(tempBuff, 0, tempBuff.Length);    //0A 55 crc crc 는 제외함
                    crcL = (byte)(crc & 0xFF);
                    crcH = (byte)((crc & 0xFF00) >> 8);

                    try
                    {
                        Debug.Log((byte)quad8.roll + "  ,  " +  (byte)quad8.pitch + "  ,  " +  (byte)quad8.yaw + "  ,  " +  (byte)quad8.throttle);
                        byte[] packetBuffer = { 0x0A, 0x55, 0x10, 0x04, 0x80, 0x10, (byte)quad8.roll, (byte)quad8.pitch, (byte)quad8.yaw, (byte)quad8.throttle, crcL, crcH };  //control::quad8 struct
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        //Debug.Log("---------- Type.control" + (byte)quad8.roll + "   " + (byte)quad8.pitch + "   " + (byte)quad8.yaw + "   " + (byte)quad8.throttle);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to sending control Packet");
                    }
                }
            }

        }

        Invoke("packetSendingHandler", _fSendInterval);


    } // packetSendingHandler()















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















    // 현재 안씀 ##########################################################################################

    // 포트 서치 ----------------------------------------------------------------------------------------------------------
    public void PortSearch()
    {
        Debug.Log("PortSearch()");

        portNames.Clear();

        switch (Application.platform) // 기기별로 포트 서치
        {
            case RuntimePlatform.OSXPlayer: // ------------------------- 애플 맥 -----------------------------
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.LinuxPlayer:
                string[] strPortNames;
//                strPortNames = System.IO.Ports.SerialPort.GetPortNames();
//
//                if (strPortNames.Length == 0)
//                {
//                    strPortNames = System.IO.Directory.GetFiles("/dev/");
//                }


                strPortNames = System.IO.Directory.GetFiles ("/dev/");

                foreach (string portName in strPortNames)
                {
                    //              if ( (portName == "/dev/cu.SLAB_USBtoUART") ||
                    //                  (portName == "/dev/cu.usbserial-AI046LHW") ||
                    //                    (portName == "/dev/cu.usbserial-1410") ||
                    //                    (portName.StartsWith ("/dev/cu.usbserial") ) ||
                    //                  (portName.StartsWith ("/dev/cu.usbmodem") ) )

                    if (portName.StartsWith ("/dev/cu."))
                        portNames.Add(portName);
                }
                break;

            default: // ---------------------------------------------- 윈도우 --------------------------------
                portNames.AddRange(SerialPort.GetPortNames());
                portName = portNames[0];
                break;
        }

        if (OnSearchCompleted != null)
            OnSearchCompleted(this, null);
    } // PortSearch


    // 선택한 포트로 연결 --------------------------------------------------------------------------------------------------------------
    public void Connect()
    {
        Debug.Log("Connect() " + portName);

        _opened = false; // 포트 열림 초기화
        _connected = false; // 실제 드론 접속 초기화
        //Debug.Log("_opened = false");

        try
        {
            _serialPort.PortName = "//./" + portName;
            _serialPort.BaudRate = baudrate;
            _serialPort.Open();
            if (_serialPort.IsOpen == true)
            {
                _opened = true;
                //Debug.Log("_opened = True");
                GetVersion_Start(); // 버전 받기 시작

                if (OnConnected != null)
                    OnConnected(this, null);
            }
            else
            {
                if (OnConnectionFailed != null)
                    OnConnectionFailed(this, null);
            }
        }
        catch (Exception)
        {
            if (OnConnectionFailed != null)
                OnConnectionFailed(this, null);
        }
    }// Connect


    // Read ----------------------------------------------------------------------------------------------------------------------
    private byte[] Read()
    {
        List<byte> bytes = new List<byte>();

        while (true)
        {
            try
            {
                bytes.Add((byte)_serialPort.ReadByte());
            }
            catch (TimeoutException)
            {
                break;
            }
            catch (Exception)
            {
                ErrorDisconnect();
                return null;
            }
        }

        if (bytes.Count == 0)
            return null;
        else
            return bytes.ToArray();
    }


    // ErrorDisconnect ----------------------------------------------------------------------------------------------------------------
    private void ErrorDisconnect()
    {
        bool state = _connected;
        _connected = false;
        _opened = false;
        Debug.Log("_opened = false");

        try
        {
            _serialPort.Close();
        }
        catch (Exception)
        {
        }

        if (state == false)
        {
            if (OnConnectionFailed != null)
                OnConnectionFailed(this, null);
        }
        else
        {
            if (OnDisconnected != null)
                OnDisconnected(this, null);
        }
    }


    // Disconnect -----------------------------------------------------------------------------------------------------------------------
    public void Disconnect()
    {
        if (_connected == true)
            _connected = false;

        _opened = false;


        ResetData();

        try
        {
            _serialPort.Close();
        }
        catch (Exception)
        {
        }


        if (OnDisconnected != null)
            OnDisconnected(this, null);
    }



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
