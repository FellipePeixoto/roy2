using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class LevelChangerTest : MonoBehaviour
{
    string savePath;
    Roy royStats;
    Klunk klunkStats;

    private void Awake() {
        savePath = Path.Combine(Application.persistentDataPath,"saveFile");
        royStats = GameObject.Find("Set_PlayerRoy").GetComponent<Roy>();
        klunkStats = GameObject.Find("Set_KLUNK").GetComponent<Klunk>();

        if(File.Exists(savePath)) {
            Vector2[] stats = LoadStats();
            royStats.SetCurrentHpSp(stats[0]);
            klunkStats.SetCurrentHpSp(stats[1]);
        }
    }

    void SaveStats() {
        using(BinaryWriter statWriter = new BinaryWriter(File.Open(savePath,FileMode.Create)) )
        {
            Vector2 RoyHpSp = royStats.GetCurrentHpSp();
            int royhp = (int)RoyHpSp.x;
            int roysp = (int)RoyHpSp.y;
            statWriter.Write(royhp);
            statWriter.Write(roysp);
            Vector2 KlunkHpSp = klunkStats.GetCurrentHpSp();
            int klunkhp = (int)KlunkHpSp.x;
            int klunksp = (int)KlunkHpSp.y;
            statWriter.Write(klunkhp);
            statWriter.Write(klunksp);
        }
    }

    Vector2[] LoadStats(){
        using(BinaryReader statReader = new BinaryReader(File.Open(savePath, FileMode.Open)) )
        {
            int royhp = statReader.ReadInt32();
            int roysp = statReader.ReadInt32();
            Vector2 roystats = new Vector2(royhp,roysp);
            int klunkhp = statReader.ReadInt32();
            int klunksp = statReader.ReadInt32();
            Vector2 klunkstats = new Vector2(klunkhp,klunksp);
            Vector2[] stats = new Vector2[] {roystats,klunkstats};
            return stats;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")){
            Debug.Log("saved");
            SaveStats();
        }
    }
}
