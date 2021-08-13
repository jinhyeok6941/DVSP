using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Ports;
using System.Text;


// ===========================================

// �κ� ������ (�ڵ��2 ����) ��â��, ����ȣ

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
            None                        = 0x00,     // ����
            Ping                        = 0x01,     // ��� Ȯ��
            Ack                         = 0x02,     // ������ ���ſ� ���� ����
            Error                       = 0x03,     // ����(reserve, ��Ʈ �÷��״� ���Ŀ� ����)
            Request                     = 0x04,     // ������ Ÿ���� ������ ��û
            Message                     = 0x05,     // ���ڿ� ������
            Address                     = 0x06,     // ��ġ �ּ�(MAC�� �ִ� ��� MAC) Ȥ�� ������ȣ(MAC�� ���� ��� UUID)
            Information                 = 0x07,     // �߿��� �� ��ġ ����
            Update                      = 0x08,     // �߿��� ������Ʈ
            UpdateLocation              = 0x09,     // �߿��� ������Ʈ ��ġ ����
            Encrypt                     = 0x0A,     // �߿��� ��ȣȭ
            SystemCount                 = 0x0B,     // �ý��� ī��Ʈ
            SystemInformation           = 0x0C,     // �ý��� ����
            Registration                = 0x0D,     // ��ǰ ���(��ȣȭ ������ �� ��� �����͸� ������ ���̷� ����)
            Administrator               = 0x0E,     // ������ ���� ȹ��
            Monitor                     = 0x0F,     // ������ �� �迭 ����. ù��° ����Ʈ�� Ÿ��, �� ��° ����Ʈ�� ������ ����(���� �޴� �������� ���� ��� ����)
            Control                     = 0x10,     // ����

            Command                     = 0x11,     // ���
            Pairing                     = 0x12,     // ��
            Rssi                        = 0x13,     // RSSI

            // Light
            LightManual                 = 0x20,     // LED ���� ����
            LightMode                   = 0x21,     // LED ���
            LightEvent                  = 0x22,     // LED ���, Ŀ�ǵ�
            LightDefault                = 0x23,     // LED ��� 3��

            RawMotion                   = 0x30,
            RawFlow,

            // ����, ����
            State                       = 0x40,     // ����
            Attitude,                               // �ڼ�
            Position,
            Altitude,                               // ��
            Motion,                                 // Motion ���� ������(IMU)
            Range,                                   // ��ġ

            // ����
            Count                       = 0x50,     // ī��Ʈ
            Bias,                                   // ����, ���̷� ���̾ ��
            Trim,                                   // Ʈ��
            Weight,                                 // ����
            LostConnection,                         // ������ ���� �� ���� �ð� ����

            // Devices
            Motor                       = 0x60,     // ���� ���� �� ���� ��� Ȯ��
            MotorSingle,                            // �� ���� ���� ����
            Buzzer,                                 // ���� ����
            Vibrator,                               // ���� ����

            // Input
            Button                      = 0x70,     // ��ư �Է�
            Joystick,                               // ���̽�ƽ �Է�

            // Display
            DisplayClear                = 0x80,     // ȭ�� �����
            DisplayInvert,                          // ȭ�� ����
            DisplayDrawPoint,                       // �� �׸���
            DisplayDrawLine,                        // �� �׸���
            DisplayDrawRect,                        // �簢�� �׸���
            DisplayDrawCircle,                      // �� �׸���
            DisplayDrawString,                      // ���ڿ� ����
            DisplayDrawStringAlign,                 // ���ڿ� �����Ͽ� ����
            DisplayDrawImage,                       // �׸� �׸���

            // Information Assembled
            InformationAssembledForController       = 0xA0,     // ���� ���ŵǴ� ������ ����(������)
            InformationAssembledForEntry            = 0xA1,     // ���� ���ŵǴ� ������ ����(���)

            NavigationTarget                        = 0xD0,     // �׺���̼� ��ǥ��
            NavigationLocation                      = 0xD1,     // �׺���̼� ���� ��ġ
            NavigationMonitor                       = 0xD2,
            NavigationHeading                       = 0xD3,
            NavigationCounter                       = 0xD4,

            GpsRtkNavigationState                   = 0xDA,     // RTK RAW ������ ����
            GpsRtkExtendedRawMeasurementData        = 0xDB,     // RTK RAW ������ ����

            EndOfType
        };
    }

    namespace CommandType
    {
        enum Type
        {
            None                        = 0x00,     // �̺�Ʈ ����

            Stop                        = 0x01,     // ����

            ModeControlFlight           = 0x02,     // ���� ���� ��� ����
            Headless                    = 0x03,     // ��帮�� ��� ����
            Trim                        = 0x04,     // Ʈ�� ����

            ClearBias                   = 0x05,     // ���̷�/���� ���̾ ����(Ʈ���� ���� �ʱ�ȭ ��)
            ClearTrim                   = 0x06,     // Ʈ�� �ʱ�ȭ

            FlightEvent                 = 0x07,     // ���� �̺�Ʈ ����

            SetDefault                  = 0x08,     // �⺻ �������� �ʱ�ȭ

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
            public byte   commandType;   // ��� Ÿ��
            public byte   option;        // ��ɿ� ���� �ɼ�
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

            Drone       = 0x10,     // ���(Server)

            Controller  = 0x20,     // ������(Client)

            Link        = 0x30,     // ��ũ ���(Client)
            LinkServer  = 0x31,     // ��ũ ���(Server, ��ũ ����� ������ �����ϴ� ��쿡�� ��� Ÿ���� ��� �ٲ�)

            ByScratch   = 0x80,     // ���̽�ũ��ġ
            Scratch     = 0x81,     // ��ũ��ġ
            Entry       = 0x82,     // ���̹� ��Ʈ��

            Tester      = 0xA0,     // �׽���
            Monitor     = 0xA1,     // �����
            Updater     = 0xA2,     // �߿��� ������Ʈ ����
            Encrypter   = 0xA3,     // ��ȣȭ ����

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
            public ushort  flags;         // Flags �������� ������ ��
            public byte  brightness;    // ���
        };

        public struct Mode
        {
            public byte  mode;       // LED ���
            public ushort  interval;   // LED ����� ���� �ֱ�
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
            public byte  evnt;      // LED �̺�Ʈ
            public ushort  interval;   // LED �̺�Ʈ ���� �ֱ�
            public byte  repeat;     // LED �̺�Ʈ �ݺ� Ƚ��
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
        public byte   dataType;       // �������� ����(sbyte : s8)
        public byte   length;         // �������� ����
        public byte   from;           // �����͸� �����ϴ� ��ġ�� DeviceType
        public byte   to;             // �����͸� �����ϴ� ��ġ�� DeviceType
    };

    public struct Ping
    {
        public ulong   systemTime;   // Ping�� �����ϴ� ��ġ�� �ð�(ulong: u64)
    };

    public struct Ack
    {
        public ulong   systemTime;     // ���� ���� �ð�
        public byte   dataType;       // ���� ���� ������ Ÿ��
        public ushort  crc16;          // ���� ���� �������� crc16(ushort : u16)
    };

    public struct Error
    {
        public ulong   systemTime;             // ���� �޼��� �۽� �ð�
        public uint    errorFlagsForSensor;    // ���� ���� �÷���
        public uint    errorFlagsForState;     // ���� ���� �÷��� (uint:u32)
    };

    public struct Request
    {
        public byte   dataType;          // ��û�� ������ Ÿ��
    };

    public struct Information
    {
        public byte   modeUpdate;     // ���� ������Ʈ ���

        public uint    modelNumber;    // �� ��ȣ
        public int    version_build;   // ���� �߿����� ���� ����
        public int    version_minor;   // ���� �߿����� ���̳� ����
        public int    version_major;   // ���� �߿����� ������ ����

        public ushort  year;           // ���� ��
        public byte   month;          // ���� ��
        public byte   day;            // ���� ��
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
        public byte  modeSystem;         // �ý��� ���
        public byte  modeFlight;         // ���� ���

        public byte  modeControlFlight;  // ���� ���� ���
        public byte  modeMovement;       // �̵� ����
        public byte  headless;           // ��帮�� ���
        public byte  controlSpeed;       // ���� �ӵ�
        public byte  sensorOrientation;  // ���� ����
        public byte  battery;            // ���͸���(0 ~ 10%)
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
        public ushort   timeNeutral;        // ���� �߸�
        public ushort   timeLanding;        // ����
        public uint     timeStop;           // ����
    };

    public struct Count
    {
        public uint  timeSystem;             // �ý��� ���� �ð�
        public uint  timeFlight;             // ���� �ð�

        public ushort  countTakeOff;           // �̷� Ƚ��
        public ushort  countLanding;           // ���� Ƚ��
        public ushort  countAccident;          // �浹 Ƚ��
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
        public byte    mode;       // ���� �۵� ���
        public ushort  value;    // Scale �Ǵ� hz
        public ushort  time;   // ���� �ð�(ms)
    };

    public struct Vibrator
    {
        public byte    mode;   // ���(0�� set, 1�� reserve)
        public ushort  on;     // ������ �� �ð�(ms)
        public ushort  off;    // ������ �� �ð�(ms)
        public ushort  total;  // ��ü ���� �ð�(ms)
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
        public sbyte  angleRoll;              // �ڼ� Roll
        public sbyte  anglePitch;             // �ڼ� Pitch
        public short  angleYaw;               // �ڼ� Yaw

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
                RearManual,             // ���� ����
                RearHold,               // ������ ������ ��� ��
                RearFlicker,            // ������
                RearFlickerDouble,      // ������(�� �� �����̰� ������ �ð���ŭ ����)
                RearDimming,            // ��� �����Ͽ� õõ�� ������

                BodyNone = 0x20,
                BodyManual,             // ���� ����
                BodyHold,               // ������ ������ ��� ��
                BodyFlicker,            // ������
                BodyFlickerDouble,      // ������(�� �� �����̰� ������ �ð���ŭ ����)
                BodyDimming,            // ��� �����Ͽ� õõ�� ������

                ANone = 0x30,
                AManual,                // ���� ����
                AHold,                  // ������ ������ ��� ��
                AFlicker,               // ������
                AFlickerDouble,         // ������(�� �� �����̰� ������ �ð���ŭ ����)
                ADimming,               // ��� �����Ͽ� õõ�� ������

                BNone = 0x40,
                BManual,                // ���� ����
                BHold,                  // ������ ������ ��� ��
                BFlicker,               // ������
                BFlickerDouble,         // ������(�� �� �����̰� ������ �ð���ŭ ����)
                BDimming,               // ��� �����Ͽ� õõ�� ������

                CNone = 0x50,
                CManual,                // ���� ����
                CHold,                  // ������ ������ ��� ��
                CFlicker,               // ������
                CFlickerDouble,         // ������(�� �� �����̰� ������ �ð���ŭ ����)
                CDimming,               // ��� �����Ͽ� õõ�� ������

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
                BodyManual,         // ���� ����
                BodyHold,           // ������ ������ ��� ��
                BodyFlicker,        // ������
                BodyFlickerDouble,  // ������(�� �� �����̰� ������ �ð���ŭ ����)
                BodyDimming,        // ��� �����Ͽ� õõ�� ������

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

        Motion_NoAnswer             = 0x00000001,   // Motion ���� ���� ����
        Motion_WrongValue           = 0x00000002,
        Motion_NotCalibrated        = 0x00000004,   // Gyro Bias ������ �Ϸ���� ����
        Motion_Calibrating          = 0x00000008,   // Gyro Bias ���� ��

        Pressure_NoAnswer           = 0x00000010,   // �з¼��� ���� ����
        Pressure_WrongValue         = 0x00000020,

        RangeGround_NoAnswer        = 0x00000100,   // �ٴ� �Ÿ����� ���� ����
        RangeGround_WrongValue      = 0x00000200,

        Flow_NoAnswer               = 0x00001000,   // Flow ���� ���� ����
        Flow_WrongValue             = 0x00002000,

        Battery_NoAnswer            = 0x00010000,   // ���͸� ���� ����
        Battery_WrongValue          = 0x00020000,
        Battery_NotCalibrated       = 0x00040000,   // ���͸� �Է°� ������ �Ϸ���� ����
    };
}// ErrorFlagsForSensor #########################################################################################################################################


// ErrorFlagsForState #########################################################################################################################################
namespace ErrorFlagsForState
{
    enum Type
    {
        None                                    = 0x00000000,

        NotRegistered                           = 0x00000001,   // ��ġ ����� �ȵ�
        FlashReadLock_UnLocked                  = 0x00000002,   // �÷��� �޸� �б� Lock�� �� �ɸ�
        BootloaderWriteLock_UnLocked            = 0x00000004,   // ��Ʈ�δ� ���� ���� Lock�� �� �ɸ�

        TakeoffFailure_CheckPropellerAndMotor   = 0x00000010,   // �̷� ����
        CheckPropellerVibration                 = 0x00000020,   // �����緯 �����߻�
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

                Attitude    = 0x10, // �ڼ� - X,Y�� ����(deg)�� �Է¹���, Z,Yaw�� �ӵ�(m/s)�� �Է� ����
                Position    = 0x11, // ��ġ - X,Y,Z,Yaw�� �ӵ�(m/s)�� �Է� ����
                Function    = 0x12, // ��� - X,Y,Z,Yaw�� �ӵ�(m/s)�� �Է� ����

                EndOfType
            };
        }
    }

    namespace System
    {
        enum Type
        {
            None = 0,

            Boot,               // ����
            Start,              // ���� �ڵ� ����
            Running,            // ���� �ڵ� ����
            ReadyToReset,       // ���� ���(1�� �� ����)
            Error,              // ����

            EndOfType
        };
    }

    namespace Flight
    {
        enum Type
        {
            None = 0,

            Ready = 0x10,       // �غ�

            Start,              // �̷� �غ�
            TakeOff,            // �̷� (Flight�� �ڵ���ȯ)
            Flight,             // ����
            Landing,            // ����
            Flip,               // ȸ��
            Reverse,            // ������

            Stop = 0x20,        // ���� ����

            Accident = 0x30,    // ��� (Ready�� �ڵ���ȯ)
            Error,              // ����

            Test = 0x40,        // �׽�Ʈ ���

            EndOfType
        };
    }

    namespace Update
    {
        enum Type
        {
            None,

            Ready,              // ������Ʈ ���� ����
            Update,             // ������Ʈ ��
            Complete,           // ������Ʈ �Ϸ�

            Faild,              // ������Ʈ ����(������Ʈ �Ϸ���� ������ body�� CRC16�� ��ġ���� �ʴ� ��� ��)

            NotAvailable,       // ������Ʈ �Ұ��� ����(//Debug ��� ��)
            RunApplication,     // ���ø����̼� ���� ��
            NotRegistered,      // ��ϵ��� ���� ��ġ

            EndOfType
        };
    }

    namespace Movement
    {
        enum Type
        {
            None        = 0x00,
            Hovering    = 0x01,     // ȣ����
            Moving      = 0x02      // �̵� ��
        };
    }
}// Mode #########################################################################################################################################


// SensorOrientation #########################################################################################################################################
namespace SensorOrientation_2
{
    enum Type
    {
        None = 0,

        Normal,             // ����
        ReverseStart,       // �������� ����
        Reversed,           // ������

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

        Headless,   // ����� �߽� ��ǥ
        Normal,     // ��� �߽� ��ǥ

        EndOfType
    };
}// Headless #########################################################################################################################################


// Trim #########################################################################################################################################
namespace Trim_2
{
    enum Type
    {
        None = 0,

        RollIncrease,       // Roll ����
        RollDecrease,       // Roll ����
        PitchIncrease,      // Pitch ����
        PitchDecrease,      // Pitch ����
        YawIncrease,        // Yaw ����
        YawDecrease,        // Yaw ����
        ThrottleIncrease,   // Throttle ����
        ThrottleDecrease,   // Throttle ����

        Reset,              // ��ü Ʈ�� ����

        EndOfType
    };
}// Trim #########################################################################################################################################


// Rotation #########################################################################################################################################
namespace Rotation
{
    enum Type
    {
        None = 0,

        Clockwise,              // �ð� ����
        Counterclockwise,       // �ݽð� ����

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
        None = 0,               // ����

        Stop = 0x10,            // ����
        TakeOff,                // �̷�
        Landing,                // ����

        Reverse,                // ������

        FlipFront,              // ȸ��
        FlipRear,               // ȸ��
        FlipLeft,               // ȸ��
        FlipRight,              // ȸ��

        Return,                 // ���� ��ġ�� ���ư���

        ResetHeading = 0xA0,    // ��� ����(�ۼַ�Ʈ ��� �� �� ���� heading�� 0���� ����)

        EndOfType
    };
}// FlightEvent #########################################################################################################################################


// Joystick #########################################################################################################################################
namespace Joystick
{
    // ���̽�ƽ ����
    namespace Direction
    {
        enum Type
        {
            None    = 0,        // �������� ���� ����(������)

            VT      = 0x10,     //   ��(����)
            VM      = 0x20,     // �߾�(����)
            VB      = 0x40,     // �Ʒ�(����)

            HL      = 0x01,     //   ����(����)
            HM      = 0x02,     //   �߾�(����)
            HR      = 0x04,     // ������(����)

            TL = 0x11,  TM = 0x12,  TR = 0x14,
            ML = 0x21,  CN = 0x22,  MR = 0x24,
            BL = 0x41,  BM = 0x42,  BR = 0x44
        };
    }

    namespace Event
    {
        enum Type
        {
            None    = 0,        // �̺�Ʈ ����

            In,                 // Ư�� ������ ����
            Stay,               // Ư�� �������� ���� ����
            Out,                // Ư�� �������� ���

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

                // ��ư
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

                // ��ư
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

        y,               // ������ ����
            Press,              // ������ ��
            Up,                 // ��

            EndContinuePress    // ���� �Է� ����
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
            Stop                = 0,    // ����(Mode������ Stop�� ��ſ��� �޾��� �� Buzzer�� ���� �뵵�� ���, set���θ� ȣ��)

            MuteInstantally     = 1,    // ���� ��� ����
            MuteContinually     = 2,    // ���� ����

            ScaleInstantally    = 3,    // ���� ��� ����
            ScaleContinually    = 4,    // ���� ����

            HzInstantally       = 5,    // ���ļ� ��� ����
            HzContinually       = 6,    // ���ļ� ����

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

            Mute    = 0xEE,     // ����
            Fin     = 0xFF      // �Ǻ��� ��
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
            Stop            = 0,    // ����

            Instantally     = 1,    // ��� ����
            Continually     = 2,    // ����

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
    public SerialPort _serialPort;
    public List<string> portNames = new List<string>(); // Ž���� ��Ʈ��
    public string portName; // ������ ��Ʈ��

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

    public int  _nSendCount_VerRC = 0; // ������ ���� ��û ���� ����Ƚ��
    public int  _nSendCount_VerRFModule = 0; // RF��� ���� ��û ���� ����Ƚ��
    public int  _nSendCount_VerDrone = 0; // ��� ���� ��û ���� ����Ƚ��



    // Information (�߿��� ����)-----------------------------------------------------------------------------------------
    public uint _nModelNumber = 0; // �޾ƿ� ������

    ushort _usVersion_Build = 0; // �޾ƿ� ������
    byte _byVersion_Minor = 0x00;
    byte _byVersion_Major = 0x00;

    ushort _usDate_Year = 0; // �޾ƿ� ������
    byte _byDate_Month = 0x00;
    byte _byDate_Day = 0x00;


    public ushort _nC_Version_build = 0;  // UI ǥ�ÿ� - ��Ʈ�ѷ� ���� ����
    public byte _nC_Version_major = 0; // UI ǥ�ÿ� - ��Ʈ�ѷ� �� ����
    public byte _nC_Version_minor = 0; // UI ǥ�ÿ� - ��Ʈ�ѷ� �� ����
    public ushort _nD_Version_build = 0;  // UI ǥ�ÿ� - ��� ���� ����
    public byte _nD_Version_major = 0; // UI ǥ�ÿ� - ��� �� ����
    public byte _nD_Version_minor = 0; // UI ǥ�ÿ� - ��� �� ����




    public int trimRoll = 0;
    public int trimPitch = 0;
    public int trimYaw = 0;
    public int trimThrottle = 0;

    public byte  lightRed = 0;
    public byte  lightGreen = 0;
    public byte  lightBlue = 0;
    public int   lightSend = 0;

    public int Flip_Front = 0; // �ø� ��
    public int Flip_Back = 0; // �ø� ��
    public int Flip_Left = 0; // �ø� ����
    public int Flip_Right = 0; // �ø� ������

    public int Count = 0; // ���� ���� ī��Ʈ �� (������ �浹 Ƚ��)



    public bool  _opened = false; // ��Ʈ ����
    public bool  _connected = false; // ���� ��� ����
    private int _sendCounter = 0;
    //private ulong _gCounter = 0;

    float _fSendInterval = 0.005f; // packetSendingHandler() �Լ� �κ�ũ ����� ����



    // �� ���� ���а� Ȱ�� (RobotConnector2��)
    public bool _bIsDetailImageFlow = false;
    public bool _bIsDetailRange = false;
    public bool _bIsDetailAxis = false;
    public bool _bIsDetailSensor = false;

    public bool _bIsAllSensor = true; // ��� ���� �����ϰ� �۵�





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

        Invoke("packetSendingHandler", 0.02f);
    }










    // ���� �ޱ� ���� ----------------------------------------------------------------------------------------------------------------
    void GetVersion_Start()
    {
        _nSendCount_VerRC = 10;
        _nSendCount_VerRFModule = 10;
        _nSendCount_VerDrone = 10;

        if ((_nC_Version_major != 0) && (_nD_Version_major != 0)) // ������, ��� ������ ��� �޾Ҵٸ�
        {
            ////Debug.Log("Got Version");
            CancelInvoke("GetVersion_Start");
        }
        else
            Invoke("GetVersion_Start", 2f);
    }




    // ���� �ޱ� -------------------------------------------------------------------------------------------------------------------
    void GetVersion(uint p_Model, byte p_Major, byte p_Minor, ushort p_Build)
    {
        string serController = "";

        if ((p_Model >= 270338) && (p_Model <= 270350)) // �ڵ�� II ������
        {
            _nC_Version_build = p_Build;
            _nC_Version_major = p_Major;
            _nC_Version_minor = p_Minor;
        }
        else if ((p_Model >= 204803) && (p_Model <= 204810)) // �ڵ�� �̴� ������
        {
            _nC_Version_build = p_Build;
            _nC_Version_major = p_Major;
            _nC_Version_minor = p_Minor;
        }
        else if ((p_Model >= 208896) && (p_Model <= 208910)) // RF ���
        {
            _nC_Version_build = p_Build;
            _nC_Version_major = p_Major;
            _nC_Version_minor = p_Minor;
        }
        else if ((p_Model >= 266245) && (p_Model <= 266260)) // �ڵ�� II
        {
            _nD_Version_build = p_Build;
            _nD_Version_major = p_Major;
            _nD_Version_minor = p_Minor;
        }
        else // �ڵ�� �̴�
        {
            _nD_Version_build = p_Build;
            _nD_Version_major = p_Major;
            _nD_Version_minor = p_Minor;
        }

        if ((_nC_Version_major != 0) && (_nD_Version_major != 0))
        {


        }
    }

    public string L_Joy = "CN" , R_Joy = "CN";
    public float L_Sense = 0 , R_Sense = 0;
    public float R_x , R_y;
    public float L_x , L_y;
    Vector3 R_dir;

    public void Debug_tempBytes()
    {
        byte[] tempBytes = Read();
        
            if (tempBytes != null)
            {
                            // if(tempBytes[2] == 0x70)
                            // {
                            //    Debug.Log(//" [0]: " + Convert.ToString(tempBytes[0], 16) +
                            //     //    " [1]: " + Convert.ToString(tempBytes[1], 16) +
                            //     //    " [2]: " + Convert.ToString(tempBytes[2], 16) +
                            //     //   " [3]: " + Convert.ToString(tempBytes[3], 16) +
                            //     //    " [4]: " + Convert.ToString(tempBytes[4], 16) +
                            //     //    " [5]: " + Convert.ToString(tempBytes[5], 16) +
                            //     //   " [6]: " + Convert.ToString(tempBytes[6], 16) +
                            //     //   " [7]: " + Convert.ToString(tempBytes[7], 16) +
                            //     //   " [8]: " + Convert.ToString(tempBytes[8], 16)); //+
                            //        //" [9]: " + Convert.ToString(tempBytes[9], 16));// +
                            //     //    " [10]: " + Convert.ToString(tempBytes[10], 16) +
                            //     //    " [11]: " + Convert.ToString(tempBytes[11], 16) +
                            //     //    " [12]: " + Convert.ToString(tempBytes[12], 16) +
                            //     //    " [13]: " + Convert.ToString(tempBytes[13], 16));
                            // }
            

                    if ((tempBytes[0] == 0x0A)&&(tempBytes[1] == 0x55))
                    {
                        int packetLength = tempBytes[3] + 8;
                        byte[] readBytes = new byte[packetLength];
                        Array.Copy(tempBytes, 0, readBytes, 0, packetLength);


                        byte crcL, crcH;
                        //byte[] testBytes = {0x0A, 0x55, 0x04, 0x01, 0xA0, 0x10, 0x40, 0x99, 0x09};
                        ushort crc = crc16_ccitt(readBytes, 2, readBytes.Length - 4);    //0A 55 crc crc �� ������
                        crcL = (byte)(crc & 0xFF);
                        crcH = (byte)((crc & 0xFF00) >> 8);

                        //Debug.Log( "crcL : " + Convert.ToString(crcL, 16) + "       crcH : " + Convert.ToString(crcH, 16) + "       crc : " + Convert.ToString(crc, 16));

                        if((crcL == readBytes[readBytes.Length - 2])&&(crcH == readBytes[readBytes.Length - 1]))
                        {
                            // Joystcik �Է°�
                            // L_JOY �Է°� 
                            // switch (readBytes[8])
                            // {
                            //     case (byte)Joystick.Direction.Type.TL:
                            //        //Debug.Log("L ���� ���");
                            //        L_Joy = "TL";
                            //        break;
                            //     case (byte)Joystick.Direction.Type.TM:
                            //        //Debug.Log("L ���");
                            //        L_Joy = "TM";
                            //        break;
                            //     case (byte)Joystick.Direction.Type.TR:
                            //        //Debug.Log("L ���� ���");
                            //        L_Joy = "TR";
                            //        break;
                            //     case (byte)Joystick.Direction.Type.ML:
                            //        //Debug.Log("L ����");
                            //        L_Joy = "ML";
                            //        break;
                            //     case (byte)Joystick.Direction.Type.CN:
                            //        //Debug.Log("L �߾�");
                            //        L_Joy = "CN";
                            //        break;
                            //     case (byte)Joystick.Direction.Type.MR:
                            //        //Debug.Log("L ����");
                            //        L_Joy = "MR";
                            //        break;
                            //     case (byte)Joystick.Direction.Type.BL:
                            //        //Debug.Log("L ���� �ϴ�");
                            //        L_Joy = "BL";
                            //        break;
                            //     case (byte)Joystick.Direction.Type.BM:
                            //        //Debug.Log("L �ϴ�");
                            //        L_Joy = "BM";
                            //        break;
                            //     case (byte)Joystick.Direction.Type.BR:
                            //        //Debug.Log("L ���� �ϴ�");
                            //        L_Joy = "BR";
                            //        break;
                            //    }

                            // switch (readBytes[12])
                            // {
                            //     case (byte)Joystick.Direction.Type.TL:
                            //        //Debug.Log("L ���� ���");
                            //        R_Joy = "TL";
                            //        break;
                            //     case (byte)Joystick.Direction.Type.TM:
                            //        //Debug.Log("L ���");
                            //        R_Joy = "TM";
                            //        break;
                            //     case (byte)Joystick.Direction.Type.TR:
                            //        //Debug.Log("L ���� ���");
                            //        R_Joy = "TR";
                            //        break;
                            //     case (byte)Joystick.Direction.Type.ML:
                            //        //Debug.Log("L ����");
                            //        R_Joy = "ML";
                            //        break;
                            //     case (byte)Joystick.Direction.Type.CN:
                            //        //Debug.Log("L �߾�");
                            //        R_Joy = "CN";
                            //        break;
                            //     case (byte)Joystick.Direction.Type.MR:
                            //        //Debug.Log("L ����");
                            //        R_Joy = "MR";
                            //        break;
                            //     case (byte)Joystick.Direction.Type.BL:
                            //        //Debug.Log("L ���� �ϴ�");
                            //        R_Joy = "BL";
                            //        break;
                            //     case (byte)Joystick.Direction.Type.BM:
                            //        //Debug.Log("L �ϴ�");
                            //        R_Joy = "BM";
                            //        break;
                            //     case (byte)Joystick.Direction.Type.BR:
                            //        //Debug.Log("L ���� �ϴ�");
                            //        R_Joy = "BR";
                            //        break;
                            //     default:
                            //        //Debug.Log(readBytes[12]);
                            //        break;
                            //    }
                            // x = Get_Sense(readBytes[10]);
                            // y = Get_Sense(readBytes[11]);
                            //Debug.Log(x + "  ,  " + y);
                            // L_Sense = Get_Sense(readBytes[6]) > Get_Sense(readBytes[7]) ? Get_Sense(readBytes[6]) : Get_Sense(readBytes[7]);
                            // R_Sense = Get_Sense(readBytes[10]) > Get_Sense(readBytes[11]) ? Get_Sense(readBytes[10]) : Get_Sense(readBytes[11]);

                            //Debug.Log(L_Sense + "  ,  " + R_Sense);
                            // Debug.Log(" [10]: " + readBytes[10] +
                            //        " [11]: " + readBytes[11] +
                            //        " [12]: " + readBytes[12] +
                            //        " [13]: " + readBytes[13] +
                            //        "  x  : " + R_x +
                            //        "  y  : " + R_y);
                            
                            if(readBytes[2] == 0x70)
                            {
                                switch(readBytes[6] + readBytes[7])
                                {
                                    // HEADLESS OFF
                                    case -1:
                                      break;
                                    // HEADLESS ON
                                    case 16:
                                      break;
                                    // M1
                                    case 32:
                                      break;
                                    // M2
                                    case 128:
                                      break;
                                    // LED , FLIP
                                    case 2:
                                      break;
                                    // Speed , Start
                                    case 1:
                                      break;
                                }
                                // Debug.Log(" [6]: " + readBytes[6] +
                                //    " [7]: " + readBytes[7] +
                                //    " [8]: " + readBytes[8]);
                            }  
                            

                            if(readBytes[2] == (byte)Protocol.DataType.Type.Joystick)
                            {
                               R_x = Get_Sense(readBytes[10]) * Check_Value(readBytes[10]);
                               R_y = Get_Sense(readBytes[11]) * Check_Value(readBytes[11]);
                               L_x = Get_Sense(readBytes[6]) * Check_Value(readBytes[6]);
                               L_y = Get_Sense(readBytes[7]) * Check_Value(readBytes[7]);
                            }    
                        }
                    }
                }
    }

    int Get_Sense(byte readbyte)
    {
        // if(readbyte > 100)
        //    return 100 - readbyte % 156;
        // else
        //    return readbyte;
        
        return readbyte > 100 ? 100 - readbyte % 156 : readbyte;
    }
    
    int Check_Value(byte readbyte)
    {
    //     if(readbyte > 100)
    //       return -1;
    //     else
    //       return 1;
          return readbyte > 100 ? -1 : 1;
    }







    // Update ------------------------------------------------------------------------------------------------------------
    // void Update()
    // {
            // ////Debug.Log(tempBytes[0] + "  |  " + tempBytes[1] + "  |  " + tempBytes[2] + "  |  " + tempBytes[3] + "  |  " + tempBytes[4]);

            // if (tempBytes != null)
            // {
            //                    for (int i = 0; i < tempBytes.Length; i++)
            //                    {
            //                        ////Debug.Log("[" + i + "] " + Convert.ToString(tempBytes[i], 16));
            //                    }
                
                
            //                    ////Debug.Log(" [0]: " + Convert.ToString(tempBytes[0], 16) +
            //                        " [1]: " + Convert.ToString(tempBytes[1], 16) +
            //                        " [2]: " + Convert.ToString(tempBytes[2], 16) +
            //                        " [3]: " + Convert.ToString(tempBytes[3], 16) +
            //                        " [4]: " + Convert.ToString(tempBytes[4], 16) +
            //                        " [5]: " + Convert.ToString(tempBytes[5], 16));
            // }
    //}


    // packetSendingHandler ----------------------------------------------------------------------------------------------------------
    private void packetSendingHandler()
    {
        //print("packetSendingHandle");
        if (_opened == true)
        {
            _sendCounter++;
            //if (_sendCounter > 12)
                _sendCounter %= 12;


            if (_sendCounter == 0) // state -------------------------------------------------------------------
            {
                //////Debug.Log("state");
                try
                {
                    byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x10, 0x40, 0x5F, 0x8F };
                    _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                    //////Debug.Log("---------- Type.state"); 
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
                //////Debug.Log("position");
                try
                {
                    byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x10, 0x42, 0x1D, 0xAF };
                    _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                    //////Debug.Log("---------- Type.position"); 
                }
                catch (Exception)
                {
                    Console.WriteLine("exceptipn : failed to sending position Packet");
                }
            }
//            else if (_sendCounter == 1) // attitude -----------------------------------------------------------
//            {
//                //////Debug.Log("attitude");
//                try
//                {
//                    byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x10, 0x41, 0x7E, 0x9F };
//                    _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
//                    //////Debug.Log("---------- Type.attitude");
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
                //////Debug.Log("altitude");
                try
                {
                    byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x10, 0x43, 0x3C, 0xBF };
                    _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                    //term.altitude = 0;
                    //////Debug.Log("---------- Type.altitude");
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
                //////Debug.Log("motion");
                try
                {
                    byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x10, 0x44, 0xDB, 0xCF };
                    _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                    //////Debug.Log("---------- Type.motion"); 
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
                //////Debug.Log("flow");
                try
                {
                    byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x10, 0x31, 0xE9, 0xE1 };
                    _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                    //////Debug.Log("---------- Type.flow"); 
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
                //////Debug.Log("range");
                try
                {
                    byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x10, 0x45, 0xFA, 0xDF };
                    _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                    //////Debug.Log("---------- Type.range"); 
                }
                catch (Exception)
                {
                    Console.WriteLine("exceptipn : failed to sending range Packet");
                }
            }
            // trim ---------------------------------------------------------------------------------------------------------------
            else if (_sendCounter == 11)
            {
                //////Debug.Log("trim");
                try
                {
                    byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x10, 0x52, 0x2C, 0xBD };
                    _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                    //////Debug.Log("---------- Type.trim"); 
                }
                catch (Exception)
                {
                    Console.WriteLine("exceptipn : failed to sending trim Packet");
                }
            }
            else // control �� ��Ÿ ��� ������ ��ȣ�� -------------------------------------------------------------------------------------
            {
                // ������ ���� ��û ���� ����-----------------------------
                if (_nSendCount_VerRC > 0)
                {
                    //////Debug.Log("VerRC");
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x20, 0x07, 0xE9, 0xB2 };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        //////Debug.Log("---------- Type.VerRC"); 
                        _nSendCount_VerRC--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to VerRC Packet");
                    }
                }

                // RF��� ���� ��û ���� ����-----------------------------
                else if (_nSendCount_VerRFModule > 0)
                {
                    //////Debug.Log("VerRFModule");
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x70, 0x30, 0x07, 0xC8, 0x52 };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        //////Debug.Log("---------- Type.VerRFModule"); 
                        _nSendCount_VerRFModule--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to VerRFModule Packet");
                    }
                }

                // ��� ���� ��û ���� ����-----------------------------
                else if (_nSendCount_VerDrone > 0)
                {
                    //////Debug.Log("VerDrone");
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x10, 0x07, 0x7C, 0xB7 };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        //////Debug.Log("---------- Type.VerDrone"); 
                        _nSendCount_VerDrone--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to VerDrone Packet");
                    }
                }
                else if (stopPressed > 0) // ����������ȣ ---------------------------------------------------------------------------
                {
                    //////Debug.Log("stop");
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x80, 0x10, 0x01, 0x00, 0xCD, 0xB6 };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        //////Debug.Log("---------- Type.stop"); 
                        stopPressed--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to stop Packet");
                    }
                }
                else if (landingPressed > 0) // ������ȣ ---------------------------------------------------------------------------
                {
                    //////Debug.Log("landing");
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x80, 0x10, 0x07, 0x12, 0x18, 0x2E };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        // ////Debug.Log("---------- Type.landing"); 
                        landingPressed--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to landing Packet");
                    }
                }
                else if (takeoffPressed > 0) // �̷���ȣ ---------------------------------------------------------------------------
                {
                    ////Debug.Log("takeoff  , " + _sendCounter);
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x80, 0x10, 0x07, 0x11, 0x7B, 0x1E };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        // ////Debug.Log("---------- Type.takeoff");
                        takeoffPressed--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to takeoff Packet");
                    }
                }
                else if (trimPressed > 0) // Ʈ����ȣ ---------------------------------------------------------------------------
                {
                    //////Debug.Log("trim");
                    byte[] Roll = BitConverter.GetBytes(trimRoll);
                    byte[] Pitch = BitConverter.GetBytes(trimPitch);
                    byte[] Yaw = BitConverter.GetBytes(trimYaw);
                    byte[] Throttle = BitConverter.GetBytes(trimThrottle);

                    byte[] tempBuff2 = { 0x52, 0x08, 0x80, 0x10, Roll[0], Roll[1], Pitch[0], Pitch[1], Yaw[0], Yaw[1], Throttle[0], Throttle[1] };
                    byte crcL2, crcH2;
                    ushort crc2 = crc16_ccitt(tempBuff2, 0, tempBuff2.Length);    //0A 55 crc crc �� ������
                    crcL2 = (byte)(crc2 & 0xFF);
                    crcH2 = (byte)((crc2 & 0xFF00) >> 8);

                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x52, 0x08, 0x80, 0x10, Roll[0], Roll[1], Pitch[0], Pitch[1], Yaw[0], Yaw[1], Throttle[0], Throttle[1], crcL2, crcH2 };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        //////Debug.Log("---------- Type.trim");
                        trimPressed--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to trim Packet");
                    }
                }
                else if (clearbiasPressed > 0) // ���� ���̾ ��ȣ ---------------------------------------------------------------------------
                {
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x80, 0x10, 0x05, 0x00, 0x09, 0x7A };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        // ////Debug.Log("---------- clear bias");
                        clearbiasPressed--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to clearbias Packet");
                    }
                }
                else if (SpeedPressed > 0) // ���ǵ� ���� ��ȣ --------------------------------------------------------------------------------
                {
                    byte[] tempBuff2 = { 0x11, 0x02, 0x80, 0x10, 0x04, (byte)_nSpeed };
                    byte crcL2, crcH2;
                    ushort crc2 = crc16_ccitt(tempBuff2, 0, tempBuff2.Length);    //0A 55 crc crc �� ������
                    crcL2 = (byte)(crc2 & 0xFF);
                    crcH2 = (byte)((crc2 & 0xFF00) >> 8);

                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x80, 0x10, 0x04, (byte)_nSpeed, crcL2, crcH2 };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        //////Debug.Log("---------- Type.Speed");
                        SpeedPressed--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to Speed Packet");
                    }
                }
//                else if (mode_linkPressed > 0) // ��� ��ũ ��ȣ ---------------------------------------------------------------------------
//                {
//                    try
//                    {
//                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x70, 0x20, 0x0A, 0x80, 0x57, 0xA1, 0xFF };
//                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
//                        // ////Debug.Log("---------- mode link");
//                        mode_linkPressed--;
//                    }
//                    catch (Exception)
//                    {
//                        Console.WriteLine("exceptipn : failed to mode_link Packet");
//                    }
//                }
//                else if (mode_controlPressed > 0) // ��� ��Ʈ�� ��ȣ ---------------------------------------------------------------------------
//                {
//                    try
//                    {
//                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x70, 0x20, 0x0A, 0x10, 0xEE, 0x22, 0xFF };
//                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
//                        // ////Debug.Log("---------- mode control");
//                        mode_controlPressed--;
//                    }
//                    catch (Exception)
//                    {
//                        Console.WriteLine("exceptipn : failed to mode_control Packet");
//                    }
//                }
                else if (lightSend > 0) // LED��ȣ ---------------------------------------------------------------------------
                {
                    //////Debug.Log("light"); //0x21 -> 0x23
                    byte[] tempBuff2 = { 0x23, 0x06, 0x80, 0x10, 0x22, 0xFF, 0x00, lightRed, lightGreen, lightBlue };
                    byte crcL2, crcH2;
                    ushort crc2 = crc16_ccitt(tempBuff2, 0, tempBuff2.Length);    //0A 55 crc crc �� ������
                    crcL2 = (byte)(crc2 & 0xFF);
                    crcH2 = (byte)((crc2 & 0xFF00) >> 8);

                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x23, 0x06, 0x80, 0x10, 0x22, 0xFF, 0x00, lightRed, lightGreen, lightBlue, crcL2, crcH2 };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        //////Debug.Log("---------- Type.Light");
                        lightSend--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to LED Packet");
                    }
                }
                else if (Flip_Front > 0) // �ø� �� ----------------------------------------------------------------------------
                {
                    //////Debug.Log("flip_front");
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x80, 0x10, 0x07, 0x14, 0xDE, 0x4E };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        // ////Debug.Log("---------- Type.Flip_Front"); 
                        Flip_Front--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to flip_front Packet");
                    }
                }
                else if (Flip_Back > 0) // �ø� �� ----------------------------------------------------------------------------
                {
                    //////Debug.Log("flip_back");
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x80, 0x10, 0x07, 0x15, 0xFF, 0x5E };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        // ////Debug.Log("---------- Type.flip_back"); 
                        Flip_Back--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to flip_back Packet");
                    }
                }
                else if (Flip_Left > 0) // �ø� �� ----------------------------------------------------------------------------
                {
                    //////Debug.Log("flip_left");
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x80, 0x10, 0x07, 0x16, 0x9C, 0x6E };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        // ////Debug.Log("---------- Type.flip_left"); 
                        Flip_Left--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to flip_left Packet");
                    }
                }
                else if (Flip_Right > 0) // �ø� �� ----------------------------------------------------------------------------
                {
                    //////Debug.Log("flip_right");
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x80, 0x10, 0x07, 0x17, 0xBD, 0x7E };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        // ////Debug.Log("---------- Type.flip_right"); 
                        Flip_Right--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to flip_right Packet");
                    }
                }
                else if (Count > 0) // ���� ���� ������ ī��Ʈ -------------------------------------------------------------------------
                {
                    //////Debug.Log("Count");
                    try
                    {
                        byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x80, 0x10, 0x50, 0x6E, 0x9D };
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        // ////Debug.Log("---------- Type.Count"); 
                        Count--;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("exceptipn : failed to Count Packet");
                    }
                }
                else // �� ������ȣ ���� !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                {
                    //////Debug.Log("control");
                    byte[] tempBuff = { 0x10, 0x04, 0x80, 0x10, (byte)quad8.roll, (byte)quad8.pitch, (byte)quad8.yaw, (byte)quad8.throttle };
                    byte crcL, crcH;
                    ushort crc = crc16_ccitt(tempBuff, 0, tempBuff.Length);    //0A 55 crc crc �� ������
                    crcL = (byte)(crc & 0xFF);
                    crcH = (byte)((crc & 0xFF00) >> 8);

                    try
                    {
                        //////Debug.Log((byte)quad8.roll + "  ,  " +  (byte)quad8.pitch + "  ,  " +  (byte)quad8.yaw + "  ,  " +  (byte)quad8.throttle);
                        byte[] packetBuffer = { 0x0A, 0x55, 0x10, 0x04, 0x80, 0x10, (byte)quad8.roll, (byte)quad8.pitch, (byte)quad8.yaw, (byte)quad8.throttle, crcL, crcH };  //control::quad8 struct
                        _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
                        //////Debug.Log("---------- Type.control" + (byte)quad8.roll + "   " + (byte)quad8.pitch + "   " + (byte)quad8.yaw + "   " + (byte)quad8.throttle);
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
    //        ushort crc = crc16_ccitt(tempBuff, 0, tempBuff.Length);    //0A 55 crc crc �� ������
    //        crcL = (byte)(crc & 0xFF);
    //        crcH = (byte)((crc & 0xFF00) >> 8);
    //
    //        ////Debug.Log(crcL + "     " + crcH);
    //    }

    //���� 

    public void Change_fSendInterval(float fSendInterval)
    {
        _fSendInterval = fSendInterval;
    }



    // ���� �Ⱦ� ##########################################################################################

    // ��Ʈ ��ġ ----------------------------------------------------------------------------------------------------------
    public void PortSearch()
    {
        ////Debug.Log("PortSearch()");

        portNames.Clear();

        switch (Application.platform) // ��⺰�� ��Ʈ ��ġ
        {
            case RuntimePlatform.OSXPlayer: // ------------------------- ���� �� -----------------------------
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

            default: // ---------------------------------------------- ������ --------------------------------
                portNames.AddRange(SerialPort.GetPortNames());
                portName = portNames[0];
                break;
        }

        if (OnSearchCompleted != null)
            OnSearchCompleted(this, null);
    } // PortSearch


    // ������ ��Ʈ�� ���� --------------------------------------------------------------------------------------------------------------
    public void Connect()
    {
        ////Debug.Log("Connect() " + portName);

        _opened = false; // ��Ʈ ���� �ʱ�ȭ
        _connected = false; // ���� ��� ���� �ʱ�ȭ
        //////Debug.Log("_opened = false");

        try
        {
            _serialPort.PortName = "//./" + portName;
            _serialPort.BaudRate = baudrate;
            _serialPort.Open();
            if (_serialPort.IsOpen == true)
            {
                _opened = true;
                //////Debug.Log("_opened = True");
                GetVersion_Start(); // ���� �ޱ� ����

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
        ////Debug.Log("_opened = false");

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







    // ������ �ʱ�ȭ ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void ResetData()
    {
        // Information (�߿��� ����)-----------------------------------------------------------------------------------------
        _nModelNumber = 0; // �޾ƿ� ������

        _usVersion_Build = 0; // �޾ƿ� ������
        _byVersion_Minor = 0x00;
        _byVersion_Major = 0x00;

        _usDate_Year = 0; // �޾ƿ� ������
        _byDate_Month = 0x00;
        _byDate_Day = 0x00;

        _nC_Version_build = 0;  // ��Ʈ�ѷ� ���� ����
        _nC_Version_major = 0; // ��Ʈ�ѷ� �� ����
        _nC_Version_minor = 0; // ��Ʈ�ѷ� �� ����
        _nD_Version_build = 0;  // ��� ���� ����
        _nD_Version_major = 0; // ��� �� ����
        _nD_Version_minor = 0; // ��� �� ����
    }







}// RobotConnector =============================================================================================================================================================
