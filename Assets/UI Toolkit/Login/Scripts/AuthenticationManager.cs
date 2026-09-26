using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;
using Unity.Services.Authentication;

public class AuthenticationManager : MonoBehaviour
{
    public static AuthenticationManager Instance {get; private set;}

    void Awake() => Instance = this;
    async Task Start()
    {
        try
        {
            await UnityServices.InitializeAsync(); 
            SetupEvents();
            
        }
        catch(Exception e)
        {
            Debug.Log(e);

        }
        
        Debug.Log($"Unity Services State:{UnityServices.State}");
    }

    public async Task<string> RegisterWithUserNamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username,password);
        }
        catch(Exception e)
        {
            Debug.LogException(e);
            return e.Message;
        }

        return "";
    }  
    public async Task<string> LoginWithUserNamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username,password);
        }
        catch(Exception e)
        {
            Debug.LogException(e);
            return e.Message;
        }

        return "";
    }


    private static void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
            Debug.Log($"Player Name: {AuthenticationService.Instance.PlayerName}");
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
          Debug.Log("Player singned out");  
        };
    }
}
