using UnityEngine;
using TMPro;                
using UnityEngine.UI;       
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement; 

public class AuthUIManager : MonoBehaviour
{
    [Header("--- STRUTTURA MENU ---")]
    public GameObject menuAutenticazione; // L'oggetto padre che contiene tutto
    public GameObject pannelloLogin;
    public GameObject pannelloRegistrazione;

    [Header("--- BOTTONE ICONA (Omino) ---")]
    public GameObject iconaLoginButton; 

    [Header("--- INPUT LOGIN ---")]
    public TMP_InputField emailLoginInput;
    public TMP_InputField passwordLoginInput;
    public TextMeshProUGUI erroreLoginText;

    [Header("--- INPUT REGISTRAZIONE ---")]
    public TMP_InputField emailRegInput;
    public TMP_InputField passwordRegInput;
    public TMP_InputField passwordConfInput; 
    public TextMeshProUGUI erroreRegText;

    async void Awake()
    {
        // 1. Spegni il menu e ACCENDI l'icona all'avvio
        if (menuAutenticazione != null) menuAutenticazione.SetActive(false);
        if (iconaLoginButton != null) iconaLoginButton.SetActive(true);

        // 2. Inizializza i servizi Unity
        try 
        {
            await UnityServices.InitializeAsync();
            
            if (AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("Utente già loggato all'avvio: " + AuthenticationService.Instance.PlayerId);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Errore Inizializzazione Servizi: " + e.Message);
        }
    }

    // --- GESTIONE APERTURA/CHIUSURA ---

    public void ApriMenuAutenticazione()
    {
        if (menuAutenticazione != null) menuAutenticazione.SetActive(true);
        if (iconaLoginButton != null) iconaLoginButton.SetActive(false);
        MostraLogin(); 
    }

    public void ChiudiTutto()
    {
        if (menuAutenticazione != null) menuAutenticazione.SetActive(false);
        if (iconaLoginButton != null) iconaLoginButton.SetActive(true);
    }

    public void MostraRegistrazione()
    {
        pannelloLogin.SetActive(false);
        pannelloRegistrazione.SetActive(true);
        if (erroreRegText) erroreRegText.text = ""; 
    }

    public void MostraLogin()
    {
        pannelloRegistrazione.SetActive(false);
        pannelloLogin.SetActive(true);
        if (erroreLoginText) erroreLoginText.text = ""; 
    }

    // --- LOGICA LOGIN (TASTO ACCEDI) ---

    public async void CliccaAccedi()
    {
        string email = emailLoginInput.text;
        string pass = passwordLoginInput.text;
        
        if (erroreLoginText) 
        {
            erroreLoginText.color = Color.yellow;
            erroreLoginText.text = "Accesso in corso...";
        }

        // Logout preventivo
        if (AuthenticationService.Instance.IsSignedIn)
        {
            AuthenticationService.Instance.SignOut();
            await Task.Delay(500); 
        }

        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(email, pass);
            
            if (erroreLoginText)
            {
                erroreLoginText.color = Color.green;
                erroreLoginText.text = "Login effettuato!";
            }
            Debug.Log("Login OK! ID: " + AuthenticationService.Instance.PlayerId);
            
            // Chiude il menu dopo il login
            Invoke("ChiudiTutto", 1f);
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError(ex.Message);
            if (erroreLoginText)
            {
                erroreLoginText.color = Color.red;
                erroreLoginText.text = "Email o password errati.";
            }
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError(ex.Message);
            if (erroreLoginText)
            {
                erroreLoginText.color = Color.red;
                erroreLoginText.text = "Errore di connessione.";
            }
        }
    }

    // --- LOGICA REGISTRAZIONE (TASTO REGISTRATI) ---

    public async void CliccaRegistrati()
    {
        string email = emailRegInput.text;
        string pass = passwordRegInput.text;
        string passConf = passwordConfInput.text;

        // 1. Controlli di sicurezza
        if (pass != passConf)
        {
            if (erroreRegText) {
                erroreRegText.color = Color.red;
                erroreRegText.text = "Le password non coincidono!";
            }
            return;
        }

        if (!ValidaPassword(pass))
        {
            if (erroreRegText) {
                erroreRegText.color = Color.red;
                erroreRegText.text = "Password debole! (Min 8 car., Maiusc, minusc, numero, simbolo)";
            }
            return;
        }

        if (erroreRegText) 
        {
            erroreRegText.color = Color.yellow;
            erroreRegText.text = "Registrazione...";
        }

        // Logout preventivo
        if (AuthenticationService.Instance.IsSignedIn)
        {
            AuthenticationService.Instance.SignOut();
            await Task.Delay(500);
        }

        // 2. Tentativo di Registrazione
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(email, pass);
            
            if (erroreRegText)
            {
                erroreRegText.color = Color.green;
                erroreRegText.text = "Registrato! Vai al Login...";
            }
            
            // --- Passaggio al pannello Login ---
            
            // 1. Copia SOLO L'EMAIL nel campo del Login
            emailLoginInput.text = email;
            
            // 2. Pulisci il campo password del Login (per sicurezza)
            passwordLoginInput.text = "";

            // 3. Manda al pannello di Login dopo 1.5 secondi
            Invoke("MostraLogin", 1.5f);
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError(ex.Message);
            if (erroreRegText)
            {
                erroreRegText.color = Color.red;
                erroreRegText.text = TraduciErrore(ex.ErrorCode);
            }
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError(ex.Message);
            if (erroreRegText)
            {
                erroreRegText.color = Color.red;
                erroreRegText.text = "Errore di connessione.";
            }
        }
    }

    // --- FUNZIONI DI SUPPORTO ---

    private bool ValidaPassword(string password)
    {
        if (password.Length < 8) return false;
        if (!Regex.IsMatch(password, @"[A-Z]")) return false; 
        if (!Regex.IsMatch(password, @"[a-z]")) return false; 
        if (!Regex.IsMatch(password, @"[0-9]")) return false; 
        if (!Regex.IsMatch(password, @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]")) return false; 
        return true;
    }

    private string TraduciErrore(long errorCode)
    {
        switch (errorCode)
        {
            case 10003: return "Email già in uso.";
            case 10004: return "Credenziali non valide.";
            case 10000: return "Richiesta non valida.";
            default: return "Errore: " + errorCode;
        }
    }
}