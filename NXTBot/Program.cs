using System;
using System.Threading;
using NKH.MindSqualls;

namespace NXTBot
{
    class Program
    {
        static NxtBrick brick;
        static NxtMotorSync motorPair;

        static void Main()
        {
            Init();
            
            brick = new NxtBrick(NxtCommLinkType.USB, 0);
            //brick = new NxtBrick(NxtCommLinkType.Bluetooth, 40);

            var sound = new NxtSoundSensor();
            var touch = new NxtTouchSensor();
            var sonar = new NxtUltrasonicSensor();

            brick.MotorA = new NxtMotor();
            brick.MotorC = new NxtMotor();
            motorPair = new NxtMotorSync(brick.MotorA, brick.MotorC);

            brick.Sensor1 = sonar;
            brick.Sensor3 = touch;
            brick.Sensor4 = sound;

            sound.PollInterval = 50;
            sound.OnPolled += sound_OnPolled;

            sonar.PollInterval = 50;
            sonar.ThresholdDistanceCm = 25;
            sonar.OnPolled += sonar_OnPolled;
            sonar.OnWithinThresholdDistanceCm += OnWithinThreshold; 
            sonar.ContinuousMeasurementCommand();

            touch.PollInterval = 50;
            touch.OnPressed += OnCollision;

            brick.Connect();

            motorPair.Run(75, 0, 0); 

            Console.WriteLine("Press any key to stop.");
            Console.ReadKey();

            brick.Disconnect();
        }

        static void Reverse()
        {
            motorPair.Brake();
            
            // set to reverse
            brick.MotorA = new NxtMotor(true);
            brick.MotorC = new NxtMotor(true);
            motorPair = new NxtMotorSync(brick.MotorA, brick.MotorC);

            motorPair.Run(75, 0, 0);

            Thread.Sleep(1000);

            motorPair.Brake();
        }

        static void OnCollision(NxtSensor sensor)
        {
            Console.WriteLine("Collided with something.");

            Reverse();
        }

        static void sound_OnPolled(NxtPollable e)
        {
            //Console.WriteLine(((NxtSoundSensor)e).Intensity);
        }
        
        static void sonar_OnPolled(NxtPollable e)
        {
            //Console.WriteLine(((NxtUltrasonicSensor)e).DistanceCm);
        }

        static void OnWithinThreshold(NxtPollable e)
        {
            Console.WriteLine("Obstacle in {0} cm", ((NxtUltrasonicSensor)e).DistanceCm);

            Reverse();
        }
        
        static void Init()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Title = "NXT BOB";
            Console.Clear();
        }
    }
}