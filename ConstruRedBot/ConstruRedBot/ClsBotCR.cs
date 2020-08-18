using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using ConstruRedBot.Classes_Folder.Users;
using ConstruRedBot.Classes_Folder.Emails;
using System.IO;
using ConstruRedBot.Classes_Folder;

namespace ConstruRedBot
{
    class ClsBotCR
    {
        
        private static TelegramBotClient Bot;
        private bool BotForWork = false;
        private bool Vivienda = false;
        private bool Remodelacion = false;
        private bool Ampliacion = false;
        private bool lamina = false;
        private bool terraza = false;
        private bool llamada = false;
        private bool prospecto = false;
        private bool Verified;
        private String BotAPI = "1162483767:AAFxxe4_hqXknGQuSDMM9c8kKhRLpsSR7hM";

        public async Task StartTelegram()
        {
            Bot = new TelegramBotClient(BotAPI);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var me = await Bot.GetMeAsync();
            Console.Title = me.Username; 

            Bot.OnMessage += BotOnReceiveMessages;
            Bot.OnMessageEdited += BotOnReceiveMessages;
            Bot.OnReceiveError += BotOnReceiveError;

            Bot.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"Listenning to Bot requests from @{me.Username}");

            Console.ReadLine();
            Bot.StopReceiving();
        }
        public async System.Threading.Tasks.Task EnviaPDFAsync(string usuario, String archivo, String titulo)
        {
            ITelegramBotClient botClient2 = new TelegramBotClient(BotAPI);
            var me = botClient2.GetMeAsync().Result;
            Console.WriteLine($"Catalogo File Sent to {usuario}");

            try
            {
                await botClient2.SendChatActionAsync(usuario, ChatAction.Typing);
                using (var fileStream = new FileStream(archivo, FileMode.Open, FileAccess.Read, FileShare.Read))
                {

                    await botClient2.SendDocumentAsync(
                        chatId: usuario,
                        caption: titulo,
                        document: new InputOnlineFile( /* content: */ fileStream, /* fileName: */ "Catalogo.pdf")
                        );
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("Error while sending PDF!" + err.Message);
            }
        }
       
        public async System.Threading.Tasks.Task EnviaFoto(string usuario, String archivo, String titulo)
        {
            var botClient = new TelegramBotClient(BotAPI);

            var me = botClient.GetMeAsync().Result;
            Console.WriteLine($"Envío de Foto a: {usuario}");


            using (var fileStream = new FileStream(archivo, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await botClient.SendPhotoAsync(
                     chatId: usuario,
                     photo: fileStream,
                     caption: titulo
                     );
            }
        }

        private async void BotOnReceiveMessages(object sender, MessageEventArgs messageEventArgs)
        {
            var ObjTelegramMessage = messageEventArgs;
            var message = ObjTelegramMessage.Message;


             List<CUsers> AllUsers = new CUsers().SendsDatatoDB();//Esta funcion envia datos de Personal de la empresa a una lista
             List<CUsers> AllCustomers = new CUsers().SendsCustDatatoDB();//Esta funcion envia datos de clientes a una lista

            string IncomingMessage="";

            string Telegram_id_message_sender = message.Chat.Id.ToString();
            string DefaultAnswer = "Lo siento, pero no logro entenderte, elije una opcion o \nescribeme informacion si deseas que te brinde informacion";
            CUsers User;
            List<CUsers> UsersList = new CUsers().SendsDatatoDB();
            User = UsersList.Find(x => x.IdBot.Equals(Telegram_id_message_sender));
            CUsers customer;
            List<CUsers> CustList = new CUsers().SendsCustDatatoDB();
            customer = CustList.Find(x => x.IdBot.Equals(Telegram_id_message_sender));
            CUsers UserWritting = AllUsers.Find(x => x.IdBot.Equals(Telegram_id_message_sender));
            CUsers CustWritting = AllCustomers.Find(x => x.IdBot.Equals(Telegram_id_message_sender));
            
            if (customer == null)
            {
                CUsers create = new CUsers();
                create.NewContactCreation(Telegram_id_message_sender);
                List<CUsers> loadcustomersagain = new CUsers().SendsCustDatatoDB();

            }//Registra el BotId si es primera vez que el usuario le escribe al bot tb_new customers
            else
            {
                DefaultAnswer = "Hola de nuevo," + customer.FullName+ "\nEs un gusto saludarte!\nYa sabes\nEscribe informacion para acceder a nuestro menu de informacion de vivienda ";
            }
            
            if (UserWritting != null)
            {
                DefaultAnswer += "Que gusto verte de nuevo por aca!" + UserWritting.Name+ " " + UserWritting.LastName;
            }//Saluda por nombre si es un colaborador de la empresa registrado
            if (message.Text == null)
            {
                IncomingMessage = "Recibiendo obj...";
            }
            else
            {
                
                IncomingMessage = message.Text.ToLower();
            }

            Console.WriteLine($"Getting message from ID {Telegram_id_message_sender}");
            Console.WriteLine($"Displaying {IncomingMessage}");

            if (IncomingMessage.Contains("start"))
            {
                DefaultAnswer = "Mucho Gusto, soy LGabBot tu asistente virtual, Empecemos con un saludo te parece! Hola!";
            }

            if (IncomingMessage.Contains("staff"))
            {
               
                CUsers VerifyUser = AllUsers.Find(x => x.IdBot.Equals(User));
                DefaultAnswer = "Ah!!! Conoces la llave!\n Bienvenido, siempre es un gusto hablar con compañeros de la empresa.\n";
                DefaultAnswer += "Por favor comparteme tu Id de la compania para poder asegurarme que eres parte de este grandioso equipo!!!";
                DefaultAnswer += "Ejemplo\n: Id 1234";
                BotForWork = true;
              
            }
            
            if (BotForWork == true)
            {
                if (IncomingMessage == null)
                {
                    DefaultAnswer = "Como laborador cuentas con las siguientes opciones:\n\nRecurso (Te envia tu catalogo de vivienda)";
                    DefaultAnswer += "\nVisita (Registra una nueva visita)\nProspecto (Te permite añadir un nuevo prospecto a la base de datos";
                }
                if (IncomingMessage.Contains("recurso"))
                {
                    DefaultAnswer = "Necesitas tu catalogo de vivienda?\n Claro con gusto!!!";
                    await new ClsBotCR().EnviaPDFAsync(Telegram_id_message_sender, "C:\\tmp\\ConstruRed\\catalogo.pdf", "Catalogo de Vivienda");
                }

                if (IncomingMessage.StartsWith("gab"))
                {
                    DefaultAnswer = "Hablas de Gab?\nMi Creador?\nSi eres tu dejame decirte que!!!\nEstas haciendo un gran trabajo sigue asi";
                }

                if (IncomingMessage.StartsWith("id"))
                {
                    string IdUser = IncomingMessage.Replace("id", "").Trim();
                    User = AllUsers.Find(x => x.IdConstruRed.Equals(IdUser));
                    if (User == null)
                    {
                        DefaultAnswer = "EL Id que ingresaste no aparece en nuestros registros, asegurate de verificar que lo escribiste correctamente";
                        DefaultAnswer += "\n Y por favor en este formato\nEjemplo: \n Id 1234 ";
                    }
                    else
                    {
                        if (!IdUser.ToLower().EndsWith("22"))
                        {
                            DefaultAnswer = "Lamento decirte que este ID no pertenece a nuestra empresa, Si nos escribias referente a informacion sobre un proyecto por favor indica que te interesa en base al primer mensaje que te envie";

                        }

                        if (new ClsVerificationEmail().FirstVerification(User).Equals("Ok"))
                        {
                            DefaultAnswer = "Hola, si tu eres *" + User.Name + User.LastName + "*, te acabo de enviar un correo de verificacion a" + User.Email + "\ncon instrucciones para registrarte";

                        }
                        else
                        {
                            DefaultAnswer = "Lo lamento hubo un inconveniente al enviar el correo de verificacion";
                        }

                        AllUsers = new CUsers().SendsDatatoDB();

                    }

                }
                
                if (IncomingMessage.ToLower().StartsWith("verificar"))
                {
                    string idUser = IncomingMessage.ToLower().Replace("verificar", "").Trim();

                    CUsers VerifyUser = AllUsers.Find(x => x.IdBot.Equals("P" + idUser));
                    if (VerifyUser != null)
                    {
                        VerifyUser.UpdateStart(VerifyUser, Telegram_id_message_sender, true);
                        DefaultAnswer = "Muchas gracias por verificar!!!"+VerifyUser.Name+" Ya te tengo registrado correctamente ahora puedes hacer uso completo del bot";
                        DefaultAnswer += "Ahora que te has registrado puedes:\n\n-Registrar una visita a un PDV, enviando tu ubicacion\n\n-Registrar un prospecto, escribe la palabra prospecto para iniciar";
                        Verified = true;
                        
                    }
                    else
                    {
                        DefaultAnswer = "Lo siento, ese codigo no es el que te envie, revisa de nuevo y envialo otra vez por favor";
                        DefaultAnswer += "\n Si ya has intentado varias veces escribe la palabra Soporte Gab Bot y un asistente se contactara contigo";
                    }
                }
                
                if (IncomingMessage.StartsWith("soporte"))
                {
                    Console.WriteLine("El usuario"+UserWritting.Name+"Quien tiene el bot id: "+UserWritting.IdBot+" Solicita ayuda");
                }
               
                if (IncomingMessage.StartsWith("visita"))
                {
                    DefaultAnswer = "Excelente para registrar tu visita inicia por enviar tu ubicacion por favor...";
                    DefaultAnswer += "\n Luego de enviar tu ubicacion envia la palabra 'PDV' espacio \n Seguido del nombre del Punto de Venta";
                }

                if (IncomingMessage.Contains("prospecto"))
                {
                    AllUsers = new CUsers().SendsDatatoDB();
                    DefaultAnswer = "Excelente, " + UserWritting.Name+" con gusto registrare tu cliente potencial\n Iniciemos con el Nombre del cliente:\nEscribe la palabra Prospecto seguido del nombre\n Ejemplo\n Haroldo Zepeda";
                    DefaultAnswer += "Luego envia el numero de telefono de la misma manera:\n Celular seguido del numero del cliente\n";
                    Console.WriteLine("Registro de Prospecto iniciado");
                    prospecto = true;
                }

                if (prospecto == true)
                {
                    if (IncomingMessage.StartsWith("prospecto"))
                    {
                        string CustomerName = IncomingMessage.ToLower().Replace("prospecto", "").Trim();
                        CUsers verificacliente = new CUsers();
                        verificacliente = AllCustomers.Find(x => x.IdBot.Equals(Telegram_id_message_sender));

                        if (CustomerName == null)
                        {
                            DefaultAnswer = "Lo lamento no pude leer el nombre de tu cliente, asegurate de enviarlo luego de la palabra prospecto";
                            DefaultAnswer = "Ejemplo:\n prospecto Mario David Lorenzana Torres";
                        }
                        else
                        {
                            verificacliente.NewProspecto(CustomerName, Telegram_id_message_sender, true);
                            DefaultAnswer = "Excelente " + UserWritting.Name + "ya tengo el nombre ahora enviame su numero de contacto de la misma manera por favor!\nEjemplo\n celular 5544-3322";

                        }
                    }
                    if (IncomingMessage.StartsWith("celular"))
                    {
                        bool registro;

                        string CustomerPhoneNumber = IncomingMessage.Replace("celular", "").Trim();
                        CUsers verificacliente = new CUsers();
                        verificacliente = AllCustomers.Find(x => x.IdBot.Equals(Telegram_id_message_sender));
                        if (CustomerPhoneNumber == null)
                        {
                            DefaultAnswer = "Lo lamento no pude leer el numero de telefono, asegurate de enviarlo luego de la palabra telefono";
                            DefaultAnswer = "Ejemplo:\n telefono 5555-5555";
                            registro = false;
                        }
                        else
                        {
                            // string CustomerName = IncomingMessage.ToLower().Replace("telefono", "").Trim();
                            DefaultAnswer = "Excelente ya tengo tu numero un asesor te estara contactando en las proximas 48 hrs\n Fue un verdadero gusto atenderte";
                            verificacliente.NewContact(CustomerPhoneNumber, Telegram_id_message_sender, false);
                            registro = true;
                            BotForWork = false;
                            lamina = false;
                            terraza = false;
                            Vivienda = false;
                            llamada = false;
                        }
                        if (registro == true)
                        {
                            await new ClsBotCR().EnviaFoto(Telegram_id_message_sender, "c:\\tmp\\ConstruRed\\Gracias.jpg", "Gracias");
                            verificacliente = AllCustomers.Find(x => x.IdBot.Equals(Telegram_id_message_sender));
                            ClsVerificationEmail Email = new ClsVerificationEmail();
                            Email.CallCustomer(verificacliente.FullName, verificacliente.PhoneNumber);
                            Console.WriteLine("Registro hecho exitosamente y correo enviado al asesor con los datos");
                        }

                    }
                }

                if (Verified == true)
                {
                    if (IncomingMessage.Contains("ubicacion"))
                    {
                        DefaultAnswer = UserWritting.Name+", si quiere registrar una ubicacion, inicie enviando su ubicacion por medio de Telegram";
                    }
                }
                

                if (ObjTelegramMessage.Message.Location != null && UserWritting != null)
                {
                     
                    GeoLocation lo = new GeoLocation();

                    
                    Console.WriteLine($"Recibiendo longitud y latitud del chat {ObjTelegramMessage.Message.Chat.Id}.");
                    Console.WriteLine($"Longitud {ObjTelegramMessage.Message.Location.Longitude}.");
                    Console.WriteLine($"Longitud {ObjTelegramMessage.Message.Location.Latitude}.");

                    String re = "Recibida y añadida su locacion"+UserWritting.Name+",\nasegurate de enviar el PDV tambien muchas gracias";

                    lo.botId = UserWritting.IdBot.ToString();
                    lo.longitude = ObjTelegramMessage.Message.Location.Longitude;
                    lo.latitude = ObjTelegramMessage.Message.Location.Latitude;
                    lo.SavesLocation(lo);

                    if (!re.Equals(""))
                    {
                        await Bot.SendTextMessageAsync(
                            chatId: ObjTelegramMessage.Message.Chat,
                            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                            text: "Gracias por mandar tu ubicacion"
                            );
                        return;
                    }

                }


                if (IncomingMessage.StartsWith("pdv"))
                {
                    string pdv = IncomingMessage.Replace("pdv", "").Trim();

                    if (pdv == null)
                    {
                        DefaultAnswer = "Lo siento pero no entiendo a que PDV te refieres\n Por favor usa este ejemplo\n PDV Ferreteria SS";
                    }
                    else
                    {
                        DefaultAnswer = "Excelente, ya registre tu visita al PDV "+pdv;
                        CUsers add = new CUsers();
                        add.AddPDV(Telegram_id_message_sender, pdv);
                        await new ClsBotCR().EnviaFoto(Telegram_id_message_sender, "c:\\tmp\\ConstruRed\\Gracias.jpg", "Gracias");
                    }
                    
                }
            }
           
            if (BotForWork == false)
            {
               
                if (IncomingMessage.Contains("llamada"))
                {
                    DefaultAnswer = "Con gusto registrare tus datos para que un asesor te contacte.\n iniciemos con tu nombre por favor enviame un mensaje con la palabra nombre seguide tu nombre completo\n";
                    DefaultAnswer += "Ejemplo:\nnombre Mario David Lorenzana Torres";
                    llamada = true;
                    AllCustomers = new CUsers().SendsCustDatatoDB();
                }
                if (IncomingMessage.Contains("informaci"))
                {

                    llamada = false;
                }
                if (llamada == false)
                {
                    if (IncomingMessage.Contains("informaci")) 
                    {
                        DefaultAnswer = "Claro, será un gusto asesorarte a realizar tu proyecto constructivo\nPodrias indicarme que tipo de proyecto te interesa:";
                        DefaultAnswer += "\nEn ConstruRed podemos asesorarte con:\n\nVivienda \nAmpliaciones\nRemodelaciones\n\n";
                        DefaultAnswer += "Por favor elije alguno de las 3 opciones y te brindare mas informacion...";
                    }
                    if (Vivienda == true)
                    {   //Ifs para registro
                        if (IncomingMessage.Contains("registrar"))
                        {
                            DefaultAnswer = "Con gusto registrare tus datos para que un asesor te contacte.\n iniciemos con tu nombre por favor enviame un mensaje con la palabra nombre seguide tu nombre completo\n";
                            DefaultAnswer += "Ejemplo:\nnombre Mario David Lorenzana Torres";
                        }//Registra nuevo contacto
                        if (IncomingMessage.StartsWith("nombre"))
                        {
                            string CustomerName = IncomingMessage.ToLower().Replace("nombre", "").Trim();
                            CUsers verificacliente = new CUsers();
                            verificacliente = AllCustomers.Find(x => x.IdBot.Equals(Telegram_id_message_sender));

                            if (CustomerName == null)
                            {
                                DefaultAnswer = "Lo lamento no pude leer tu nombre, asegurate de enviarlo luego de la palabra nombre";
                                DefaultAnswer = "Ejemplo:\n nombre Mario David Lorenzana Torres";
                            }
                            else
                            {
                                verificacliente.NewContact(CustomerName, Telegram_id_message_sender, true);
                                DefaultAnswer = "Excelente ya tengo tu nombre ahora enviame tu numero de contacto de la misma manera por favor!\nEjemplo\n celular 5544-3322";

                            }
                        }
                        if (IncomingMessage.StartsWith("celular"))
                        {
                            bool registro;

                            string CustomerPhoneNumber = IncomingMessage.Replace("celular", "").Trim();
                            CUsers verificacliente = new CUsers();
                            verificacliente = AllCustomers.Find(x => x.IdBot.Equals(Telegram_id_message_sender));
                            if (CustomerPhoneNumber == null)
                            {
                                DefaultAnswer = "Lo lamento no pude leer tu numero de telefono, asegurate de enviarlo luego de la palabra telefono";
                                DefaultAnswer = "Ejemplo:\n nombre Mario David Lorenzana Torres";
                                registro = false;
                            }
                            else
                            {
                                // string CustomerName = IncomingMessage.ToLower().Replace("telefono", "").Trim();
                                DefaultAnswer = "Excelente ya tengo tu numero un asesor te estara contactando en las proximas 48 hrs\n Fue un verdadero gusto atenderte";
                                verificacliente.NewContact(CustomerPhoneNumber, Telegram_id_message_sender, false);
                                registro = true;
                                BotForWork = false;
                                lamina = false;
                                terraza = false;
                                Vivienda = false;
                                llamada = false;
                            }
                            if (registro == true)
                            {
                                await new ClsBotCR().EnviaFoto(Telegram_id_message_sender, "c:\\tmp\\ConstruRed\\Gracias.jpg", "Gracias");
                                verificacliente = AllCustomers.Find(x => x.IdBot.Equals(Telegram_id_message_sender));
                                AllCustomers = new CUsers().SendsCustDatatoDB();
                                
                            }
                            if (registro == true)
                            {
                                ClsVerificationEmail Email = new ClsVerificationEmail();
                                Email.CallCustomer(verificacliente.FullName, verificacliente.PhoneNumber);
                                Console.WriteLine("Registro hecho exitosamente y correo enviado al asesor con los datos");
                            }
                        }

                        //tipo de techo
                        if (IncomingMessage.Contains("lamina"))
                        {
                            DefaultAnswer = "Excelente, cuantas habitaciones te gustarian?\n2?\n3?\n4?";
                            lamina = true;
                        } 

                        if (lamina == true)
                        {
                            if (IncomingMessage.Contains("2"))
                            {
                                DefaultAnswer = "Ok, con gusto te comparto nuestros modelos con 2 habitaciones...";
                                DefaultAnswer += "\nAdjunto te comparto algunos modelos de casas habitat las cuales cuentan con cuotas desde Q.850.00 mensuales con una tasa de interes anual del 10%";
                                DefaultAnswer += "\nCada casa cuenta con su nombre si tienes duda de alguna enviame el nombre y te dire su informacion detallada...";
                                await new ClsBotCR().EnviaPDFAsync(Telegram_id_message_sender, "C:\\tmp\\ConstruRed\\catalogo.pdf", "Catalogo de Vivienda");
                            }


                            if (IncomingMessage.Contains("3"))
                            {
                                DefaultAnswer = "Ok, con gusto te comparto nuestros modelos con 3 habitaciones...";
                                DefaultAnswer += "\nCada casa cuenta con su nombre si tienes duda de alguna enviame el nombre y te dire su informacion detallada...";
                                await new ClsBotCR().EnviaPDFAsync(Telegram_id_message_sender, "C:\\tmp\\ConstruRed\\catalogo.pdf", "Catalogo de Vivienda");
                            }
                            
                            
                            if (IncomingMessage.Contains("4"))
                            {
                                DefaultAnswer = "Dejame decirte que no poseemos una casa de catalogo con 4 habitaciones\n sin embargo eso no nos impide diseñartela...";
                                DefaultAnswer += "Por favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                                DefaultAnswer += "Envia la palabra 'registrar' para iniciar con el proceso";
                            }

                            
                        }
                        if (IncomingMessage.Contains("terraza"))
                        {
                            DefaultAnswer = "Excelente,cuantas habitaciones te gustarian? ";
                            terraza = true;
                        }
                        if (terraza == true)
                        {
                            if (IncomingMessage.Contains("2"))
                            {
                                DefaultAnswer = "Ok, con gusto te comparto nuestros modelos con 2 habitaciones...";
                                DefaultAnswer += "\nCada casa cuenta con su nombre si tienes duda de alguna enviame el nombre y te dire su informacion detallada...";
                                await new ClsBotCR().EnviaPDFAsync(Telegram_id_message_sender, "C:\\tmp\\ConstruRed\\catalogo.pdf", "Catalogo de Vivienda");
                            }
                            if (IncomingMessage.Contains("3"))
                            {
                                DefaultAnswer = "Ok, con gusto te comparto nuestros modelos con 3 habitaciones...";
                                DefaultAnswer += "\nCada casa cuenta con su nombre si tienes duda de alguna enviame el nombre y te dire su informacion detallada...";
                                await new ClsBotCR().EnviaPDFAsync(Telegram_id_message_sender, "C:\\tmp\\ConstruRed\\catalogo.pdf", "Catalogo de Vivienda");
                            }
                            if (IncomingMessage.Contains("4"))
                            {
                                DefaultAnswer = "Dejame decirte que no poseemos una casa de catalogo con 4 habitaciones\n sin embargo eso no nos impide diseñartela...";
                                DefaultAnswer += "Por favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                                DefaultAnswer += "Envia la palabra 'registrar' para iniciar con el proceso";
                            
                            }
                        }

                        //Informacion de casas
                        if (IncomingMessage.Contains("habitat"))
                        {
                            DefaultAnswer = "Dejame decirte que las casas habitat son una excelente opcion en cuanto a economia\nSi deseas informacion de algun modelo\n";
                            DefaultAnswer += "Asegurate de escribir su nombre: \n Ejemplo: Tipo 10 o tipo 1";
                        }
                        if (IncomingMessage.Contains("tipo 1"))
                        {
                            DefaultAnswer = "La Casa Tipo 1 es una excelente opcion debido a sus cuotas y tasa de interes, te comento un poco...\n";
                            DefaultAnswer += "\nM2 de Construccion: 49.08\nHabitaciones: 2\nSala-comedor\nCocina\nY cuenta con 1 sanitario\n";
                            DefaultAnswer += "\nIdeal para un terreno de 7X10 o mayor o tambien la podemos adaptar...";
                            DefaultAnswer += "Si deseas mas informacion \nPor favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                            DefaultAnswer += "\nEnvia la palabra 'registrar' para iniciar con el proceso";
                        }
                        if (IncomingMessage.Contains("tipo 2"))
                        {
                            DefaultAnswer = "La Casa Tipo 2 es una excelente opcion debido a sus cuotas y tasa de interes, te comento un poco...\n";
                            DefaultAnswer += "\nM2 de Construccion: 49.08\nHabitaciones: 2\nSala-comedor\nCocina\nY cuenta con 1 sanitario\n";
                            DefaultAnswer += "\nIdeal para un terreno de 7X10 o mayor o tambien la podemos adaptar...";
                            DefaultAnswer += "Si deseas mas informacion \nPor favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                            DefaultAnswer += "\nEnvia la palabra 'registrar' para iniciar con el proceso";
                        }
                        if (IncomingMessage.Contains("tipo 10"))
                        {
                            DefaultAnswer = "La Casa Tipo 10 es una excelente opcion debido a sus cuotas bajas y tasa de interes, te comento un poco...\n";
                            DefaultAnswer += "\nHabitaciones: 3\nSala-comedor\nCocina\nY cuenta con 1 sanitario\n";
                            DefaultAnswer += "\nIdeal para un terreno de lotificacion o mayor o tambien la podemos adaptar...";
                            DefaultAnswer += "Si deseas mas informacion \nPor favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                            DefaultAnswer += "\nEnvia la palabra 'registrar' para iniciar con el proceso";
                        }
                        if (IncomingMessage.Contains("margarita"))
                        {
                            await new ClsBotCR().EnviaFoto(Telegram_id_message_sender, "c:\\tmp\\ConstruRed\\margarita.jpg", "Casa Margarita");
                            DefaultAnswer = "La Casa Margarita es una de las mas vendidas, te comento un poco...\n";
                            DefaultAnswer += "\nM2 de Construccion: 77\nHabitaciones: 3\nSala-comedor\nCocina\nY cuenta con 1 sanitario y area verde\n";
                            DefaultAnswer += "\nSu distribucion nos asegura que cada ambiente quede bien ventilado...";
                            DefaultAnswer += "Si deseas mas informacion \nPor favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                            DefaultAnswer += "\nEnvia la palabra 'registrar' para iniciar con el proceso";
                        }
                        if (IncomingMessage.Contains("violeta"))
                        {
                            await new ClsBotCR().EnviaFoto(Telegram_id_message_sender, "c:\\tmp\\ConstruRed\\violeta.jpg", "Casa violeta");
                            DefaultAnswer = "La Casa Violeta es una excelente opcion y la podemos hacer ya sea de lamina o terraza, te comento un poco...\n";
                            DefaultAnswer += "\nM2 de Construccion: 46\nHabitaciones: 2\nSala-comedor\nCocina\nY cuenta con 1 sanitario\n";
                            DefaultAnswer += "\nSu distribucion nos asegura que cada ambiente quede bien ventilado...";
                            DefaultAnswer += "Si deseas mas informacion \nPor favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                            DefaultAnswer += "\nEnvia la palabra 'registrar' para iniciar con el proceso";
                        }
                        if (IncomingMessage.Contains("azucena"))
                        {
                            DefaultAnswer = "La Casa azucena es una excelente opcion , te comento un poco...\n";
                            DefaultAnswer += "\nM2 de Construccion: 42\nHabitaciones: 2\nSala-comedor\nCocina\nY cuenta con 1 sanitario\n";
                            DefaultAnswer += "\nSu distribucion nos asegura que cada ambiente quede bien ventilado...";
                            DefaultAnswer += "Si deseas mas informacion \nPor favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                            DefaultAnswer += "\nEnvia la palabra 'registrar' para iniciar con el proceso";
                        }
                        if (IncomingMessage.Contains("rural"))
                        {
                            DefaultAnswer = "La Casa rural es una excelente opcion , te comento un poco...\n";
                            DefaultAnswer += "\nM2 de Construccion: 45\nHabitaciones: 3\nSala-comedor\nCocina\nY cuenta con 1 sanitario\n";
                            DefaultAnswer += "\nSu distribucion nos asegura que cada ambiente quede bien ventilado...";
                            DefaultAnswer += "Si deseas mas informacion \nPor favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                            DefaultAnswer += "\nEnvia la palabra 'registrar' para iniciar con el proceso";
                        }
                        if (IncomingMessage.Contains("jazmin"))
                        {
                            DefaultAnswer = "La Casa jazmin es una excelente opcion , te comento un poco...\n";
                            DefaultAnswer += "\nM2 de Construccion: 45\nHabitaciones: 2\nSala exterior\nY cuenta con 1 sanitario\n";
                            DefaultAnswer += "\nSu distribucion nos asegura que cada ambiente quede bien ventilado...";
                            DefaultAnswer += "Si deseas mas informacion \nPor favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                            DefaultAnswer += "\nEnvia la palabra 'registrar' para iniciar con el proceso";
                        }
                        if (IncomingMessage.Contains("azalea"))
                        {
                            await new ClsBotCR().EnviaFoto(Telegram_id_message_sender, "c:\\tmp\\ConstruRed\\azalea.jpg", "Casa azalea");
                            DefaultAnswer = "La Casa azucena es una excelente opcion con techo de losa, te comento un poco...\n";
                            DefaultAnswer += "\nM2 de Construccion: 62\nHabitaciones: 3\nSala-comedor\nCocina\nY cuenta con 1 sanitario y su area verde\n";
                            DefaultAnswer += "\nSu distribucion nos asegura que cada ambiente quede bien ventilado...";
                            DefaultAnswer += "Si deseas mas informacion \nPor favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                            DefaultAnswer += "\nEnvia la palabra 'registrar' para iniciar con el proceso";
                        }
                        if (IncomingMessage.Contains("orquidea"))
                        {
                            await new ClsBotCR().EnviaFoto(Telegram_id_message_sender, "c:\\tmp\\ConstruRed\\orquidea.jpg", "Casa orquidea");
                            DefaultAnswer = "La Casa orquidea es una excelente opcion con techo de losa, te comento un poco...\n";
                            DefaultAnswer += "\nM2 de Construccion: 67\nHabitaciones: 3\nSala-comedor\nCocina\nY cuenta con 1 sanitario y su area verde\n";
                            DefaultAnswer += "\nSu distribucion nos asegura que cada ambiente quede bien ventilado...";
                            DefaultAnswer += "Si deseas mas informacion \nPor favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                            DefaultAnswer += "\nEnvia la palabra 'registrar' para iniciar con el proceso";
                        }
                    }
                    if (Ampliacion == true)
                    {   //Tipos de Ampliacion
                        if (IncomingMessage.Contains("habitacion"))
                        {
                            DefaultAnswer = "Como nos comprenderas este tema es un poco extenso...\n";
                            DefaultAnswer += "Si deseas una mejor asesoria en este tema \nPor favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                            DefaultAnswer += "\nEnvia la palabra 'registrar' para iniciar con el proceso";
                        }
                        if (IncomingMessage.Contains("bano"))
                        {
                            DefaultAnswer = "Como nos comprenderas este tema es un poco extenso...\n";
                            DefaultAnswer += "Si deseas una mejor asesoria en este tema \nPor favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                            DefaultAnswer += "\nEnvia la palabra 'registrar' para iniciar con el proceso";
                        }
                        if (IncomingMessage.Contains("loca"))
                        {
                            DefaultAnswer = "Como nos comprenderas este tema es un poco extenso...\n";
                            DefaultAnswer += "Si deseas una mejor asesoria en este tema \nPor favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                            DefaultAnswer += "\nEnvia la palabra 'registrar' para iniciar con el proceso";
                        }

                        //ifs para registro
                        if (IncomingMessage.Contains("registrar"))
                        {
                            DefaultAnswer = "Con gusto registrare tus datos para que un asesor te contacte.\n iniciemos con tu nombre por favor enviame un mensaje con la palabra nombre seguide tu nombre completo\n";
                            DefaultAnswer += "Ejemplo:\nnombre Mario David Lorenzana Torres";
                        }//Registra nuevo contacto
                        if (IncomingMessage.StartsWith("nombre"))
                        {
                            string CustomerName = IncomingMessage.ToLower().Replace("nombre", "").Trim();
                            CUsers verificacliente = new CUsers();
                            verificacliente = AllCustomers.Find(x => x.IdBot.Equals(Telegram_id_message_sender));

                            if (CustomerName == null)
                            {
                                DefaultAnswer = "Lo lamento no pude leer tu nombre, asegurate de enviarlo luego de la palabra nombre";
                                DefaultAnswer = "Ejemplo:\n nombre Mario David Lorenzana Torres";
                            }
                            else
                            {
                                verificacliente.NewContact(CustomerName, Telegram_id_message_sender, true);
                                DefaultAnswer = "Excelente ya tengo tu nombre ahora enviame tu numero de contacto de la misma manera por favor!\nEjemplo\n celular 5544-3322";

                            }
                        }
                        if (IncomingMessage.StartsWith("celular"))
                        {
                            bool registro;

                            string CustomerPhoneNumber = IncomingMessage.Replace("celular", "").Trim();
                            CUsers verificacliente = new CUsers();
                            verificacliente = AllCustomers.Find(x => x.IdBot.Equals(Telegram_id_message_sender));
                            if (CustomerPhoneNumber == null)
                            {
                                DefaultAnswer = "Lo lamento no pude leer tu numero de telefono, asegurate de enviarlo luego de la palabra telefono";
                                DefaultAnswer = "Ejemplo:\n nombre Mario David Lorenzana Torres";
                                registro = false;
                            }
                            else
                            {
                                // string CustomerName = IncomingMessage.ToLower().Replace("telefono", "").Trim();
                                DefaultAnswer = "Excelente ya tengo tu numero un asesor te estara contactando en las proximas 48 hrs\n Fue un verdadero gusto atenderte";
                                verificacliente.NewContact(CustomerPhoneNumber, Telegram_id_message_sender, false);
                                registro = true;
                                BotForWork = false;
                                lamina = false;
                                terraza = false;
                                Vivienda = false;
                                llamada = false;
                            }
                            if (registro == true)
                            {
                                await new ClsBotCR().EnviaFoto(Telegram_id_message_sender, "c:\\tmp\\ConstruRed\\Gracias.jpg", "Gracias");
                                verificacliente = AllCustomers.Find(x => x.IdBot.Equals(Telegram_id_message_sender));
                                AllCustomers = new CUsers().SendsCustDatatoDB();
                                ClsVerificationEmail Email = new ClsVerificationEmail();
                                Email.CallCustomer(verificacliente.FullName, verificacliente.PhoneNumber);
                                Console.WriteLine("Registro hecho exitosamente y correo enviado al asesor con los datos");
                            }

                        }
                    }
                    if (Remodelacion == true)
                    {   //Tipos de Remodelacion
                        if (IncomingMessage.Contains("concreto"))
                        {
                        DefaultAnswer = "Como nos comprenderas este tema es un poco extenso...\n";
                        DefaultAnswer += "Si deseas una mejor asesoria en este tema \nPor favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                        DefaultAnswer += "\nEnvia la palabra 'registrar' para iniciar con el proceso";
                        }
                        if (IncomingMessage.Contains("terraza"))
                        {
                            DefaultAnswer = "Como nos comprenderas este tema es un poco extenso...\n";
                            DefaultAnswer += "Si deseas una mejor asesoria en este tema \nPor favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                            DefaultAnswer += "\nEnvia la palabra 'registrar' para iniciar con el proceso";
                        }
                        if (IncomingMessage.Contains("repellar"))
                        {
                            DefaultAnswer = "Como nos comprenderas este tema es un poco extenso...\n";
                            DefaultAnswer += "Si deseas una mejor asesoria en este tema \nPor favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                            DefaultAnswer += "\nEnvia la palabra 'registrar' para iniciar con el proceso";
                        }
                        if (IncomingMessage.Contains("repello"))
                        {
                            DefaultAnswer = "Como nos comprenderas este tema es un poco extenso...\n";
                            DefaultAnswer += "Si deseas una mejor asesoria en este tema \nPor favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                            DefaultAnswer += "\nEnvia la palabra 'registrar' para iniciar con el proceso";
                        }
                        if (IncomingMessage.Contains("lamina"))
                        {
                            DefaultAnswer = "Como nos comprenderas este tema es un poco extenso...\n";
                            DefaultAnswer += "Si deseas una mejor asesoria en este tema \nPor favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                            DefaultAnswer += "\nEnvia la palabra 'registrar' para iniciar con el proceso";
                        }
                        if (IncomingMessage.Contains("piso"))
                        {
                            DefaultAnswer = "Como nos comprenderas este tema es un poco extenso...\n";
                            DefaultAnswer += "Si deseas una mejor asesoria en este tema \nPor favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                            DefaultAnswer += "\nEnvia la palabra 'registrar' para iniciar con el proceso";
                        }
                        if (IncomingMessage.Contains("ceramico"))
                        {
                            DefaultAnswer = "Como nos comprenderas este tema es un poco extenso...\n";
                            DefaultAnswer += "Si deseas una mejor asesoria en este tema \nPor favor registra tus datos y un asesor te estara contactando a la brevedad posible.\n solo necesito tu nombre y numero de telefono";
                            DefaultAnswer += "\nEnvia la palabra 'registrar' para iniciar con el proceso";
                        }

                        //ifs para registro
                        if (IncomingMessage.Contains("registrar"))
                        {
                            DefaultAnswer = "Con gusto registrare tus datos para que un asesor te contacte.\n iniciemos con tu nombre por favor enviame un mensaje con la palabra nombre seguide tu nombre completo\n";
                            DefaultAnswer += "Ejemplo:\nnombre Mario David Lorenzana Torres";
                        }//Registra nuevo contacto
                        if (IncomingMessage.StartsWith("nombre"))
                        {
                            string CustomerName = IncomingMessage.ToLower().Replace("nombre", "").Trim();
                            CUsers verificacliente = new CUsers();
                            verificacliente = AllCustomers.Find(x => x.IdBot.Equals(Telegram_id_message_sender));

                            if (CustomerName == null)
                            {
                                DefaultAnswer = "Lo lamento no pude leer tu nombre, asegurate de enviarlo luego de la palabra nombre";
                                DefaultAnswer = "Ejemplo:\n nombre Mario David Lorenzana Torres";
                            }
                            else
                            {
                                verificacliente.NewContact(CustomerName, Telegram_id_message_sender, true);
                                DefaultAnswer = "Excelente ya tengo tu nombre ahora enviame tu numero de contacto de la misma manera por favor!\nEjemplo\n celular 5544-3322";

                            }
                        }
                        if (IncomingMessage.StartsWith("celular"))
                        {
                            bool registro;

                            string CustomerPhoneNumber = IncomingMessage.Replace("celular", "").Trim();
                            CUsers verificacliente = new CUsers();
                            verificacliente = AllCustomers.Find(x => x.IdBot.Equals(Telegram_id_message_sender));
                            if (CustomerPhoneNumber == null)
                            {
                                DefaultAnswer = "Lo lamento no pude leer tu numero de telefono, asegurate de enviarlo luego de la palabra telefono";
                                DefaultAnswer = "Ejemplo:\n nombre Mario David Lorenzana Torres";
                                registro = false;
                            }
                            else
                            {
                                // string CustomerName = IncomingMessage.ToLower().Replace("telefono", "").Trim();
                                DefaultAnswer = "Excelente ya tengo tu numero un asesor te estara contactando en las proximas 48 hrs\n Fue un verdadero gusto atenderte";
                                verificacliente.NewContact(CustomerPhoneNumber, Telegram_id_message_sender, false);
                                registro = true;
                                BotForWork = false;
                                lamina = false;
                                terraza = false;
                                Vivienda = false;
                                llamada = false;
                            }
                            if (registro == true)
                            {
                                await new ClsBotCR().EnviaFoto(Telegram_id_message_sender, "c:\\tmp\\ConstruRed\\Gracias.jpg", "Gracias");
                                verificacliente = AllCustomers.Find(x => x.IdBot.Equals(Telegram_id_message_sender));
                                AllCustomers = new CUsers().SendsCustDatatoDB();
                                ClsVerificationEmail Email = new ClsVerificationEmail();
                                Email.CallCustomer(verificacliente.FullName, verificacliente.PhoneNumber);
                                Console.WriteLine("Registro hecho exitosamente y correo enviado al asesor con los datos");
                            }

                        }
                    }

                    //Opciones del menu que activan las de arriba
                    if (IncomingMessage.Contains("vivienda"))
                    {
                        DefaultAnswer = "Okay, podrias decirme que tipo de casa quieres?\nEmpecemos por el techo\nTe gustaria:\n\nLamina?\nO\n Terraza?";
                        Vivienda = true;
                    }

                    if (IncomingMessage.Contains("ampliaci"))
                    {
                        DefaultAnswer = "Excelente, podrias decirme que tipo de ampliacion quisieras realizar?\n";
                        DefaultAnswer += "\nHabitaciones?\nUn nuevo Baño?\nLocal Comercial?\n\n";
                        Ampliacion = true;

                    }

                    if (IncomingMessage.Contains("remodelaci"))
                    {
                        DefaultAnswer = "Excelente, podrias decirme que tipo de Remodelacion quisieras realizar?\n";
                        DefaultAnswer += "\nTecho de Concreto?\nRepellar?\nTecho de Lamina?\nPiso Ceramico?\n\n";
                        Remodelacion= true;

                    }
                }
                if (llamada == true)
                {
                   
                    if (UserWritting == null)
                    {

                        DefaultAnswer = "Con gusto registrare tus datos para que un asesor te contacte.\n iniciemos con tu nombre por favor enviame un mensaje con la palabra nombre seguido de tu nombre completo\n";
                        DefaultAnswer += "Ejemplo:\nnombre Mario David Lorenzana Torres";
                    }
                    if (IncomingMessage.StartsWith("nombre"))
                    {
                        string CustomerName = IncomingMessage.ToLower().Replace("nombre", "").Trim();
                        CUsers verificacliente = new CUsers();
                        verificacliente = AllCustomers.Find(x => x.IdBot.Equals(Telegram_id_message_sender));

                        if (CustomerName == null)
                        {
                            DefaultAnswer = "Lo lamento no pude leer tu nombre, asegurate de enviarlo luego de la palabra nombre";
                            DefaultAnswer = "Ejemplo:\n nombre Mario David Lorenzana Torres";
                        }
                        else
                        {
                            verificacliente.NewContact(CustomerName, Telegram_id_message_sender, true);
                            DefaultAnswer = "Excelente ya tengo tu nombre ahora enviame tu numero de contacto de la misma manera por favor!\nEjemplo\n celular 5544-3322";
                           
                        }
                    }
                    if (IncomingMessage.StartsWith("celular"))
                    {
                        bool registro;
                        
                        string CustomerPhoneNumber = IncomingMessage.Replace("celular", "").Trim();
                        CUsers verificacliente = new CUsers();
                        verificacliente = AllCustomers.Find(x => x.IdBot.Equals(Telegram_id_message_sender));
                        if (CustomerPhoneNumber == null)
                        {
                            DefaultAnswer = "Lo lamento no pude leer tu numero de telefono, asegurate de enviarlo luego de la palabra telefono";
                            DefaultAnswer = "Ejemplo:\n nombre Mario David Lorenzana Torres";
                            registro = false;
                        }
                        else
                        {
                           // string CustomerName = IncomingMessage.ToLower().Replace("telefono", "").Trim();
                            DefaultAnswer = "Excelente ya tengo tu numero un asesor te estara contactando en las proximas 48 hrs\n Fue un verdadero gusto atenderte";
                            verificacliente.NewContact(CustomerPhoneNumber, Telegram_id_message_sender, false);
                            registro = true;
                            BotForWork = false;
                            lamina = false;
                            terraza = false;
                            Vivienda = false;
                            llamada = false;
                        }
                        if (registro == true)
                        {
                            await new ClsBotCR().EnviaFoto(Telegram_id_message_sender, "c:\\tmp\\ConstruRed\\Gracias.jpg", "Gracias");
                            verificacliente = AllCustomers.Find(x => x.IdBot.Equals(Telegram_id_message_sender));
                            AllCustomers = new CUsers().SendsCustDatatoDB();
                            ClsVerificationEmail Email = new ClsVerificationEmail();
                            Email.CallCustomer(verificacliente.FullName, verificacliente.PhoneNumber);
                            Console.WriteLine("Registro hecho exitosamente y correo enviado al asesor con los datos");
                        }
                        
                    }
                    
                }
            }

            if (IncomingMessage.Contains("prestamo"))
            {
                DefaultAnswer = "Te interesa financiamiento para tu proyecto, claro!!!\nDejame decirte que nosotros podemos ayudarte!!!\n Solo necesito que te registres\n\n";
                DefaultAnswer += "Con gusto registrare tus datos para que un asesor te contacte.\n iniciemos con tu nombre por favor enviame un mensaje con la palabra nombre seguido de tu nombre completo\n";
                DefaultAnswer += "Ejemplo:\nnombre Mario David Lorenzana Torres";
            }

            if (IncomingMessage.Contains("construred"))
            {
                DefaultAnswer = "La Mejor Compañía en Guatemala\nNo lo supiste de mi claro\nPero dicen que manejan unos modelos de casa inigualables!!!\nSi no tienen el modelo que buscas te lo diseñan!!! GRATIS!!!";
                DefaultAnswer += "\n\nEs mas si quieres te puedo contar un poco sobre lo que ofrecen!!!\n Solo escribeme la palabra informacion";
            }

            if (IncomingMessage.Contains("facebook"))
            {
                Console.WriteLine("Pagina de Fb Compartida");
                DefaultAnswer = "Ya nos sigues en Facebook? Aun no? Que esperas???\nSiguenos hoy mismo\nhttps://www.facebook.com/ConstruRedOficial";
            }


            if (IncomingMessage.Contains("catalogo"))
            {
                DefaultAnswer = "Deseas nuestro catalogo?\n Con gusto te lo comparto";
                Console.WriteLine("Catalogo enviado al usuario");
                await new ClsBotCR().EnviaPDFAsync(Telegram_id_message_sender, "C:\\tmp\\ConstruRed\\catalogo.pdf", "Catalogo de Vivienda");
               // respuesta = "Hola " + alumno.nombre + " " + alumno.apellido + " Te mando un libro, espero que te sirva";
            }


            if (IncomingMessage.Contains("gracias"))
            {
                DefaultAnswer = "Gracias a ti por usar mis servicios fue un verdadero gusto!!!\nPero no te vayas sin registrarte";
                DefaultAnswer += "Con gusto registrare tus datos para que un asesor te contacte.\n iniciemos con tu nombre por favor enviame un mensaje con la palabra nombre seguido de tu nombre completo\n";
                DefaultAnswer += "Ejemplo:\nnombre Mario David Lorenzana Torres";
            }


            if (IncomingMessage.Contains("hola"))
            { 
                DefaultAnswer = "Hola mucho gusto, soy LGabBot y sera un honor darte a conocer nuestras soluciones constructivas y beneficios!!!";
                DefaultAnswer += "\nCuentame, quieres que te explique o prefieres una llamada por uno de nuestros asesores?\nPor favor respondeme con una de estas palabras";
                DefaultAnswer += "\nInformación\no\nLlamada";
                await new ClsBotCR().EnviaFoto(Telegram_id_message_sender, "c:\\tmp\\GabBot.jpg", "Hi");
            }

            

            if (!String.IsNullOrEmpty(DefaultAnswer))//    
            {
                await Bot.SendTextMessageAsync(
                    chatId: ObjTelegramMessage.Message.Chat,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    text: DefaultAnswer

            );
            }


        }

        public static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Ups!!! Recibio un error!!! {0} - {1} :",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message
                );
        }
    }
}
