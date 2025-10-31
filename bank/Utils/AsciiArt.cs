using System;
using System.Threading;
using System.Threading.Tasks;

namespace bank.Utils
{
    public static class AsciiArt
    {
        /// <summary>
        /// Splash screen with piggy bank - shows once at startup with 2 second timer
        /// </summary>
        public static void DisplaySplashScreen()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
                                             #                                                       
                                            %#@                                                      
                                           @##%@@    **                                              
                                          @*##%%@@  @@@@   @@@@                                      
                                     @ @   @@##@   @#@@   @@@                                        
                                 %  ++ +@@@@##*%%@@@@@@@@ @@                                         
                             @@@%%##  @  ++**#@@%@@@@ @@@@@@                                         
                                @###%+  @@+***# %@@@@@@@@@@#                                         
                                  @#*+    =**@@@@@@@@@@@@@@                                          
                                   @*+ *@@%***##%%@@@@@@@@@@                                         
                                    @@     %@@@@@@@@@# %@@@@ @@                                      
                                     *=%@@#*@@@@@@%%@@@@  @@@@@                                      
                                   #@@%##@@@%***++  =%@@@      *@@                                   
                                  @@ +***#@@@@@@@@@@@       **   *@                                  
                                @@@ +****@@*###@@@@@  +   =@@@@     #@                               
                              %@@@   @#*@@**%@@@@@@@@   +              @@                            
                           %@@@@@@@@@ +++@**@@@*@@@#*#**+++       %@@@@@@                            
                            @@@@@@@@  @@@#@****@@##@@*****++    *      *@@                           
                              @@@@@% *@@@@++*+*@*@@@@@**#@@@@@ @ #     #@@                           
                              @@@@@  @@@@ ++@+**#@@@@@@@@@@+@@*@     @@@@@                           
                              @@@@@  @@@   @@ ++@@@@@@@@@@@@@@*++@@@     @                           
                              @@*@@+ @@@   @@ ++@@@@@@@@@@@@@@@ *+++     @                           
                              @@@#@#*+@@   @@@  +@@@ @@@@@@@@@@@@+*  +*-*@                           
                              @@@ %#***@+   @@@   @@++ *@@@@@@@@@@@@@@@@@@                           
                              %@@@ ###**++   @@@    =++++#@@@@@@@@@@@@@@                             
                               @@@@  ###@*++  @@@@    + @#@@@@@@@@  @@@                              
                               +@@@@#   @@# *   @@@@@#  @@@@@ @@@@  @@                               
                                @@@@@@* @@@@@    #@@@@@@@@@@@* @@@                                   
                                 @@@@@@@@@@@@@@@*   @@@@@@@@@@ *@                                    
                                   @  @@@@@@@@@@@@@@@@@@@@@@@@  +                                    
                                        @@@@@     @@@@@@@@@@@@                                       
                                            @@*       @@@@@@@*                                       
                                                        @@@@@                                        
                                                         @@@                                         
                                                          @                                          
");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("                         MALMÖ ROYAL BANK");
            Console.WriteLine("                      Your Trust, Our Priority");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n                       Powered by Team Malmö\n");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                 Press any key to continue...");
            Console.ResetColor();

            // Wait 2 seconds or until key press
            var cts = new CancellationTokenSource();
            var keyTask = Task.Run(() => Console.ReadKey(true), cts.Token);

            if (Task.WaitAny(keyTask, Task.Delay(2000)) == 1)
            {
                // Timeout reached, consume any pending key press
                cts.Cancel();
            }
        }

        /// <summary>
        /// Compact MRB logo for menu screens
        /// </summary>
        public static void DisplayBankLogo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@" 
+------------------------------------+
| ____    ____  _______     ______   |
||_   \  /   _||_   __ \   |_   _ \  |
|  |   \/   |    | |__) |    | |_) | |
|  | |\  /| |    |  __ /     |  __'. |
| _| |_\/_| |_  _| |  \ \_  _| |__) ||
||_____||_____||____| |___||_______/ |
+------------------------------------+                                                     
");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("MALMÖ ROYAL BANK");
            Console.WriteLine("Your Trust, Our Priority");
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}