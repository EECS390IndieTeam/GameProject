using UnityEngine;
using UnityEngine.UI;
using System.Collections;

 public class Timer : MonoBehaviour {
     public Text timer;
     public bool countDown;
     public int secondsToCountDown;
     public int timeLimit;
 
     private float time;
 
     void Start(){
     	if (countDown){
     		time = (secondsToCountDown);
     	}
     }

     void Update() {
         if (countDown){
         	if (time > 0) {	
         		time -= Time.deltaTime;
         	} else {
         		time = 0;
         	}

     	} else {
     		if (time < timeLimit){
     			time += Time.deltaTime;
     		} else {
     			time = timeLimit;
     		}
     	}
         
 
         var minutes = time / 60; //Divide by sixty to get the minutes.
         var seconds = time % 60; //mod 60 for the seconds.
         var fraction = (time * 100) % 100;
 
         //update the label value
         if (time > 60){
            timer.text = string.Format ( "{0:00}:{1:00}", (int) minutes, (int) seconds);       //casting to int necessary to avoid float math issues
         } else {
            timer.text = string.Format("{0:00}", (int) seconds);
         }
         
     }

 }
