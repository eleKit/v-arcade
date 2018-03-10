using UnityEngine;
using System.Collections;

public class TuningInfos : MonoBehaviour {
	
	//static string first = "Ciao, " + SaveInfos.userName +"!\nPrima di giocare, registriamo alcune informazioni.\nMetti le mani sul leap, con i palmi rivolti verso il basso,\na circa 20cm l'una dall'altra.";
	//static string ago = "Ciao, " + SaveInfos.userName +"!\nE' da un po' che non aggiorniamo le tue informazioni.\nMetti le mani sul leap, con i palmi rivolti verso il basso,\na circa 20cm l'una dall'altra.";
	static string wait = "Bene! Mantieni la posizione per cinque secondi.";
	static string redo = "Ok, si ricomincia!";
	static string handsUp = "Inclina piu' che puoi le mani verso l'alto e\nmantieni la posizione per 5 secondi.";
	static string handsDown = "Ora inclina piu' che puoi le mani verso il basso e\nmantieni la posizione per 5 secondi.";
	static string handsRight = "Bene! Inclina piu' che puoi le mani a destra e\nmantieni la posizione per 5 secondi.";
	static string handsLeft = "Ci siamo quasi!\nInclina piu' che puoi le mani a sinistra e\nmantieni la posizione per 5 secondi.";
	static string end = "\nOttimo! Ora puoi iniziare a giocare!\nOppure puoi reimpostare le informazioni.\n\nPer tenere conto dei tuoi progressi ti verra' chiesto di\naggiornare le tue informazioni una volta alla settimana.";
	static string incoherentData = "Qualcosa non va con i dati acquisiti.\nPremi aggiorna per riprovare."; 

	public void SetFirstTimeText(){
		string first = "Ciao, " + PlayerSaveData.playerData.GetUserName() +"!\nPrima di giocare, registriamo alcune informazioni.\nMetti le mani sul leap, con i palmi rivolti verso il basso,\na circa 20cm l'una dall'altra.";
		GetComponent<TextMesh>().text = first;
	}

	public void SetLongAgoText(){
		string ago = "Ciao, " + PlayerSaveData.playerData.GetUserName() +"!\nE' da un po' che non aggiorniamo le tue informazioni.\nMetti le mani sul leap, con i palmi rivolti verso il basso,\na circa 20cm l'una dall'altra.";
		GetComponent<TextMesh>().text = ago;
	}

	public void SetRedoText(){
		GetComponent<TextMesh>().text = redo;
	}

	public void SetWaitText(){
		GetComponent<TextMesh>().text = wait;
	}

	public void SetIncoherentText(){
		GetComponent<TextMesh>().text = incoherentData;
	}

	public void SetHandsUpText(){
		GetComponent<TextMesh>().text = handsUp;
	}

	public void SetHandsDownText(){
		GetComponent<TextMesh>().text = handsDown;
	}

	public void SetHandsLeftText(){
		GetComponent<TextMesh>().text = handsLeft;
	}

	public void SetHandsRightText(){
		GetComponent<TextMesh>().text = handsRight;
	}

	public void SetEndText(){
		GetComponent<TextMesh>().text = end;
	}
}
