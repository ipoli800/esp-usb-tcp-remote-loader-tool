


//public class clnt
//{

//    public static void Main()
//    {

//        try
//        {
//            String str;
//            TcpClient tcpclnt = new TcpClient();
//            Console.WriteLine("Connecting.....");

//            tcpclnt.Connect("10.22.0.80", 80); // use the ipaddress as in the server program

//            Console.WriteLine("Connected");
//            Console.Write("Enter the string to be transmitted : ");
//            do
//            {
//                str = Console.ReadLine();
//                Stream stm = tcpclnt.GetStream();

//                ASCIIEncoding asen = new ASCIIEncoding();
//                byte[] ba = asen.GetBytes(str);
//                Console.WriteLine("Transmitting.....");

//                stm.Write(ba, 0, ba.Length);

//                byte[] bb = new byte[100];
//                int k = stm.Read(bb, 0, 100);

//                for (int i = 0; i < k; i++)
//                    Console.Write(Convert.ToChar(bb[i]));
//            }
//            while (str != "exit");

//                        tcpclnt.Close();
//        }

//        catch (Exception e)
//        {
//            Console.WriteLine("Error..... " + e.StackTrace);
//        }
//    }

//}