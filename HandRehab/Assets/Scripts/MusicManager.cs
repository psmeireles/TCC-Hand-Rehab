using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public List<AudioClip> songs;
    public AudioSource audioSource;
    int currentSong = 0;
    // Start is called before the first frame update
    void Start() {
        songs = Randomize(songs).ToList();
    }

    // Update is called once per frame
    void Update()
    {
        if(songs.Count > 0 && !audioSource.isPlaying) {
            currentSong = (currentSong + 1) % songs.Count;
            audioSource.clip = songs[currentSong];
            audioSource.Play();
        }
    }

    public static IEnumerable<T> Randomize<T>(IEnumerable<T> source) {
        return source.OrderBy<T, int>((item) => Random.Range(0, source.Count() - 1));
    }
}
