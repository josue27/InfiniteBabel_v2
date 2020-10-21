using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.UI;
using TMPro;

public class LogIn : MonoBehaviour
{
    public Text signInButtonText;
    public Text authStatus;
    // Start is called before the first frame update
    void Start()
    {
         //  ADD THIS CODE BETWEEN THESE COMMENTS

       // // Create client configuration
       // PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();

    
       
       // // Enable debugging output (recommended)
       // PlayGamesPlatform.DebugLogEnabled = true;
        
       // // Initialize and activate the platform
       // PlayGamesPlatform.InitializeInstance(config);
       // PlayGamesPlatform.Activate();
       //// END THE CODE TO PASTE INTO START

       //   // PASTE THESE LINES AT THE END OF Start()
       // // Try silent sign-in (second parameter is isSilent)
       // PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);
    }
    //98:1E:3A:07:08:2E:A4:D8:6E:3E:81:29:E1:1F:DB:36:38:4A:A8:F3

    public void SignIn() {
        //Debug.Log("signInButton clicked!");

        // if (!PlayGamesPlatform.Instance.localUser.authenticated) {
        //    // Sign in with Play Game Services, showing the consent dialog
        //    // by setting the second parameter to isSilent=false.
        //    PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
        //} else {
        //    // Sign out of play games
        //    PlayGamesPlatform.Instance.SignOut();
            
        //    // Reset UI
        //    signInButtonText.text = "Sign In";
        //    authStatus.text = "";
        //}
    }

    public void SignInCallback(bool success) {

        if (success) {
            Debug.Log("(Lollygagger) Signed in!");
            
            // Change sign-in button text
            signInButtonText.text = "Sign out";
            
            // Show the user's name
           // authStatus.text = "Signed in as: " + Social.localUser.userName;
        } else {
            Debug.Log("(Lollygagger) Sign-in failed...");
            
            // Show failure message
            signInButtonText.text = "Sign in";
            authStatus.text = "Sign-in failed";
        }
    }
}
