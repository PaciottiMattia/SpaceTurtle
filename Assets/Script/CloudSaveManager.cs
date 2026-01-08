using UnityEngine;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using System.Threading.Tasks;

public class CloudSaveManager : MonoBehaviour
{
    // QUESTA è la riga magica che mancava o non era salvata!
    public static CloudSaveManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- SALVA IL PUNTEGGIO ---
    public async Task SalvaRecord(int nuovoPunteggio)
    {
        var dati = new Dictionary<string, object> {
            { "HighScore", nuovoPunteggio } 
        };

        try
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(dati);
            Debug.Log("Nuovo Record salvato nel cloud: " + nuovoPunteggio);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Errore salvataggio: " + e.Message);
        }
    }

    // --- CARICA IL PUNTEGGIO ---
    public async Task<int> CaricaRecord()
    {
        try
        {
            var datiSalvati = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "HighScore" });

            if (datiSalvati.ContainsKey("HighScore"))
            {
                string json = datiSalvati["HighScore"].Value.GetAsString();
                
                // Tentativo di conversione sicura
                if (int.TryParse(json, out int record))
                {
                    return record;
                }
                else 
                {
                    return System.Convert.ToInt32(datiSalvati["HighScore"].Value.GetAs<int>());
                }
            }
            else
            {
                return 0; // Nessun record trovato
            }
        }
        catch (System.Exception e)
        {
            // Ignora errori se l'utente non è loggato
            return 0;
        }
    }
}