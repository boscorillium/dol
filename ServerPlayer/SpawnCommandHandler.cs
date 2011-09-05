// -----------------------------------------------------------------------
// <copyright file="SpawnCommandHandler.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using DOL.GS.PacketHandler;
using DOL.GS.PacketHandler.Client.v168;
using DOL.SP;
	
namespace ServerPlayer
{
    using DOL.Database;
    using DOL.GS;
    using DOL.GS.Commands;
    using System.Threading;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Cmd("&spawn", 
        ePrivLevel.Player,
        "/spawn character_name account_name password"
        )]
    public class SpawnCommandHandler : AbstractCommandHandler, ICommandHandler
    {
        #region ICommandHandler Members

        private static Controller controller;

        public void OnCommand(DOL.GS.GameClient client, string[] args)
        {           
            if (args.Length == 3)
            {
                string characterName = args[1];
                string accountName = args[2];
                //string password = args[3];

                Account playerAccount = GameServer.Database.FindObjectByKey<Account>(accountName);
                if (playerAccount == null )
                    return;

                if (controller == null)
                {
                    //controller = new Controller(client.Player);
                    controller = Controller.Instance;
                    controller.MainCharacter = client.Player;
                    new Thread(controller.Start).Start();
                }

                ServerGameClient gameClient = new ServerGameClient(null);
                gameClient.Account = playerAccount;

                WorldMgr.CreateSessionID(gameClient);

                int charIndex = gameClient.Account.GetCharacterIndexByName(characterName);
                if (charIndex != -1)
                {
                    var pLib = new FakePacketLib(client.Player);
                    gameClient.Out = pLib;
                    gameClient.LoadPlayer(charIndex);
                    pLib.Player = gameClient.Player;
                    //gameClient.Player.ObjectState = GameObject.eObjectState.Active;

                    new WorldInitRequestHandler.WorldInitAction(gameClient.Player).Start(1);

                    new PlayerInitRequestHandler().HandlePacket(gameClient, null);

                    controller.players.Add(gameClient.Player);
                    
                    GamePlayer p = client.Player;
                    gameClient.Player.MoveTo(p.CurrentRegionID, p.X, p.Y, p.Z, p.Heading);

                    /*
                    var fpa = new FollowPlayerAction(gameClient.Player, client.Player);
                    fpa.Interval = 50;
                    fpa.Start(1);
                     */

                    /*
                    if (gameClient.Player.CharacterClass.Name.ToLower() == "cleric")
                    {
                        var ba = new BuffGroupAction(gameClient.Player);
                        ba.Interval = 100;
                        ba.Start(1);
                    }
                    else
                    {
                        var fa = new FightGroupAction(gameClient.Player);
                        fa.Interval = 100;
                        fa.Start(1);
                    }
                     */
                }

                //new WorldInitRequestHandler.WorldInitAction(

                /*
				client.Out.SendMessage("This will spawn a character from your account", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("Trying to spawn " + args[1], eChatType.CT_System, eChatLoc.CL_SystemWindow);
				
                GameClient gc = new GameClient(null);
                gc.Account = client.Account;                

                int charIndex = gc.Account.GetCharacterIndexByName(args[1]);
                if (charIndex != -1)
                {					
					var pLib = new FakePacketLib(client.Player);
                    gc.Out = pLib;
                    gc.LoadPlayer(charIndex);
                    pLib.Player = gc.Player;
					
					new WorldInitRequestHandler.WorldInitAction(gc.Player).Start(1);
					
					var fpa = new FollowPlayerAction(gc.Player, client.Player);
					fpa.Interval = 50;
					fpa.Start(1);

                    if (gc.Player.CharacterClass.Name.ToLower() == "cleric")
                    {
                        var ba = new BuffGroupAction(gc.Player);
                        ba.Interval = 100;
                        ba.Start(1);
                    }
                    else
                    {
                        var fa = new FightGroupAction(gc.Player);
                        fa.Interval = 100;
                        fa.Start(1);
                    }
                 
                }
                 */
            }
            else
            {
                DisplaySyntax(client);
                return;
            }
        }

        #endregion
    }
}
