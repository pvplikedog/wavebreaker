using System;
using System.Collections.Generic;
using Pathfinding.Util;
using UnityEngine;

namespace Pathfinding
{
    [HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_animation_link.php")]
    public class AnimationLink : NodeLink2
    {
        public string clip;
        public float animSpeed = 1;
        public bool reverseAnim = true;

        public GameObject referenceMesh;
        public LinkClip[] sequence;
        public string boneRoot = "bn_COG_Root";

        public override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            var buffer = ListPool<Vector3>.Claim();
            var endPosition = Vector3.zero;
            CalculateOffsets(buffer, out endPosition);
            Gizmos.color = Color.blue;
            for (var i = 0; i < buffer.Count - 1; i++) Gizmos.DrawLine(buffer[i], buffer[i + 1]);
        }

        private static Transform SearchRec(Transform tr, string name)
        {
            var childCount = tr.childCount;

            for (var i = 0; i < childCount; i++)
            {
                var ch = tr.GetChild(i);
                if (ch.name == name) return ch;
                var rec = SearchRec(ch, name);
                if (rec != null) return rec;
            }

            return null;
        }

        public void CalculateOffsets(List<Vector3> trace, out Vector3 endPosition)
        {
            //Vector3 opos = transform.position;
            endPosition = transform.position;
            if (referenceMesh == null) return;

            var ob = Instantiate(referenceMesh, transform.position, transform.rotation);
            ob.hideFlags = HideFlags.HideAndDontSave;

            var root = SearchRec(ob.transform, boneRoot);
            if (root == null) throw new Exception("Could not find root transform");

            var anim = ob.GetComponent<Animation>();
            if (anim == null) anim = ob.AddComponent<Animation>();

            for (var i = 0; i < sequence.Length; i++) anim.AddClip(sequence[i].clip, sequence[i].clip.name);

            var prevOffset = Vector3.zero;
            var position = transform.position;
            var firstOffset = Vector3.zero;

            for (var i = 0; i < sequence.Length; i++)
            {
                var c = sequence[i];
                if (c == null)
                {
                    endPosition = position;
                    return;
                }

                anim[c.clip.name].enabled = true;
                anim[c.clip.name].weight = 1;

                for (var repeat = 0; repeat < c.loopCount; repeat++)
                {
                    anim[c.clip.name].normalizedTime = 0;
                    anim.Sample();
                    var soffset = root.position - transform.position;

                    if (i > 0)
                        position += prevOffset - soffset;
                    else
                        firstOffset = soffset;

                    for (var t = 0; t <= 20; t++)
                    {
                        var tf = t / 20.0f;
                        anim[c.clip.name].normalizedTime = tf;
                        anim.Sample();
                        var tmp = position + (root.position - transform.position) + c.velocity * tf * c.clip.length;
                        trace.Add(tmp);
                    }

                    position = position + c.velocity * 1 * c.clip.length;

                    anim[c.clip.name].normalizedTime = 1;
                    anim.Sample();
                    var eoffset = root.position - transform.position;
                    prevOffset = eoffset;
                }

                anim[c.clip.name].enabled = false;
                anim[c.clip.name].weight = 0;
            }

            position += prevOffset - firstOffset;

            DestroyImmediate(ob);

            endPosition = position;
        }

        [Serializable]
        public class LinkClip
        {
            public AnimationClip clip;
            public Vector3 velocity;
            public int loopCount = 1;

            public string name => clip != null ? clip.name : "";
        }
    }
}