using System;
using UnityEngine;
using UnityEngine.UIElements;

public class LoginController
{
    private VisualElement root;
    private Button loginButton;
    private Button registerButton;
    private TextField userNameField;
    private TextField passwordField;
    
    // Essa Action é como um "telefone" para ligar para o UIManager
    private Action onLoginSuccess; 

    // O construtor recebe a UI e o método que deve ser chamado quando o login der certo
    public LoginController(VisualElement rootVisualElement, Action onLoginSuccessCallback)
    {
        root = rootVisualElement;
        onLoginSuccess = onLoginSuccessCallback;
        ConfigurarTela();
    }

    private void ConfigurarTela()
    {
        loginButton = root.Q<Button>("loginButton");
        registerButton = root.Q<Button>("registerButton");
        userNameField = root.Q<TextField>("emailTextField");
        passwordField = root.Q<TextField>("passwordTextField");

        loginButton.clicked += LoginButton_Clicked;
        registerButton.clicked += RegisterButton_Clicked;
    }

    private async void LoginButton_Clicked()
    {
        registerButton.SetEnabled(false);
        loginButton.SetEnabled(false);

        try
        {
            // Olha o seu AuthenticationManager sendo chamado aqui de forma limpa!
            var errorText = await AuthenticationManager.Instance.LoginWithUserNamePasswordAsync(
                userNameField.value, 
                passwordField.value
            );
            
            if (string.IsNullOrEmpty(errorText))
            {
                Debug.Log("Login aprovado! Avisando o UIManager...");
                
                // "Liga" para o UIManager e avisa: "Pode trocar a tela!"
                onLoginSuccess?.Invoke(); 
            }
            else
            {
                Debug.LogError($"Falha no Login: {errorText}");
                ReabilitarBotoes();
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            ReabilitarBotoes();
        }
    }

    private async void RegisterButton_Clicked()
    {
        registerButton.SetEnabled(false);
        loginButton.SetEnabled(false);

        try
        {
            var errorText = await AuthenticationManager.Instance.RegisterWithUserNamePasswordAsync(
                userNameField.value, 
                passwordField.value
            );
            Debug.Log(errorText);
        }
        finally
        {
            ReabilitarBotoes();
        }
    }

    private void ReabilitarBotoes()
    {
        registerButton.SetEnabled(true);
        loginButton.SetEnabled(true);
    }
}