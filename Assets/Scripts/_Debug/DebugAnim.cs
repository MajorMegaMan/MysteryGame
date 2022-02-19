using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

public class DebugAnim : MonoBehaviour
{
    public Animator anim;
    public PlayableGraph playableGraph;

    public AnimationClip clip;

    // Start is called before the first frame update
    void Start()
    {
        playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        var playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", anim);

        // Wrap the clip in a playable
        var clipPlayable = AnimationClipPlayable.Create(playableGraph, clip);

        // Connect the Playable to an output
        playableOutput.SetSourcePlayable(clipPlayable);

        // Plays the Graph.
        playableGraph.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            anim.Play("Attack", 0, 0.0f);
        }
    }

    void RelayEvent()
    {
        anim.Play("Idle");
    }

    private void OnEnable()
    {
        playableGraph = PlayableGraph.Create();
    }

    private void OnDisable()
    {
        // Destroys all Playables and PlayableOutputs created by the graph.
        playableGraph.Destroy();
    }
}
