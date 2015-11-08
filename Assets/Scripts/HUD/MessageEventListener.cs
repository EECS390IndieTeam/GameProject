using UnityEngine;
using System.Collections;

public class MessageEventListener : Bolt.GlobalEventListener {

	public override void OnEvent(MessageTextEvent evnt){

		if(BoltNetwork.isServer){
			MessageTextEvent sevent = MessageTextEvent.Create(Bolt.GlobalTargets.AllClients, Bolt.ReliabilityModes.ReliableOrdered);
			sevent.Message = evnt.Message;
			sevent.Send();
		}
	}
}
