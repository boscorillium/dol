// -----------------------------------------------------------------------
// <copyright file="SpawnCommandHandler.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using DOL.GS.PacketHandler;
using DOL.SP;
using DOL.GS.PacketHandler.Client.v168;
	
namespace ServerPlayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using DOL.GS;
    using DOL.GS.Commands;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Cmd("&spawn", 
        ePrivLevel.Player,
        "/spawn character_name"
        )]
    public class SpawnCommandHandler : AbstractCommandHandler, ICommandHandler
    {
        #region ICommandHandler Members

        public void OnCommand(DOL.GS.GameClient client, string[] args)
        {           
            if (args.Length == 2)
            {
				client.Out.SendMessage("This will spawn a character from your account", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("Trying to spawn " + args[1], eChatType.CT_System, eChatLoc.CL_SystemWindow);
				
                GameClient gc = new GameClient(null);
                gc.Account = client.Account;                

                int charIndex = gc.Account.GetCharacterIndexByName(args[1]);
                if (charIndex != -1)
                {
                    gc.LoadPlayer(charIndex);	
					
					gc.Out = new FakePacketLib(gc.Player, client.Player);
					
					new WorldInitRequestHandler.WorldInitAction(gc.Player).Start(1);
					
					var fpa = new FollowPlayerAction(gc.Player, client.Player);
					fpa.Interval = 500;
					fpa.Start(1);
                }
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
