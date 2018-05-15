using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace baobao
{
    /// <summary>
    /// ビルトインで用意されているTrackとClipに対応したタイムラインヘルパークラス
    /// </summary>
    [RequireComponent(typeof(PlayableDirector))]
    public class DefaultTimelineHelper : MonoBehaviour
    {
        protected IEnumerable<TrackAsset> _tracks;

        PlayableDirector _director;

        protected PlayableDirector director
        {
            get
            {
                if (_director == null)
                    _director = GetComponent<PlayableDirector>();
                return _director;
            }
        }

        public PlayState State
        {
            get { return director.state; }
        }

        #region TimelineControl

        public void Pause()
        {
            director.Pause();
        }

        public void Play()
        {
            director.Play();
        }

        public void Stop()
        {
            director.Stop();
        }

        public void Resume()
        {
            director.Resume();
        }

        public void SetTimeUpdateMode(DirectorUpdateMode mode)
        {
            director.timeUpdateMode = mode;
        }

        public void Evaluate()
        {
            director.Evaluate();
        }

        public void SetTime(float time, bool withEvaluate = false)
        {
            director.time = (double) time;
            if (withEvaluate)
                Evaluate();
        }

        public float GetFPS()
        {
            var t = (TimelineAsset) director.playableAsset;
            return t.editorSettings.fps;
        }


        public void SetPlayable(TimelineAsset asset)
        {
            director.playableAsset = asset;
        }

        #endregion

        #region Track

        /// <summary>
        /// AnimationTrackターゲットバインド
        /// </summary>
        public void BindAnimationTrackTarget(string trackName, Animator bindObj)
        {
            BindTrackTarget(trackName, bindObj);
        }

        /// <summary>
        /// ActivationTrackのターゲットバインド
        /// </summary>
        public void BindActivationTrackTarget(string trackName, GameObject bindObj)
        {
            BindTrackTarget(trackName, bindObj);
        }

        /// <summary>
        /// AudioTrackのAudioSourceバインド
        /// </summary>
        public void BindAudioTrackTarget(string trackName, AudioSource bindObj)
        {
            BindTrackTarget(trackName, bindObj);
        }

        /// <summary>
        /// 指定トラックにバインド
        /// </summary>
        public void BindTrackTarget<T>(string trackName, T target) where T : Object
        {
            var track = GetTrackList().FirstOrDefault(x => x.name == trackName);
            director.SetGenericBinding(track, target);
        }

        #endregion

        #region Clip

        /// <summary>
        /// ControlPlayableClipの中身を更新
        /// </summary>
        public void SetControlPlayableClipValue(string trackName, string clipName, GameObject obj)
        {
            var clip = GetClip<ControlPlayableAsset>(trackName, clipName);
            if (clip != null)
            {
                // ExposedReferenceを差し替える
                director.SetReferenceValue(clip.sourceGameObject.exposedName, obj);
            }
        }

        /// <summary>
        /// AudioPlayableの中身を更新
        /// </summary>
        public void SetAudioPlayableClipValue(string trackName, string clipName, AudioClip obj)
        {
            var clip = GetClip<AudioPlayableAsset>(trackName, clipName);
            if (clip != null)
            {
                clip.clip = obj;
            }

            foreach (var a in clip.outputs)
            {
                Debug.Log(a);
            }
        }

        #endregion

        /// <summary>
        /// クリップを取得
        /// </summary>
        T GetClip<T>(string trackName, string clipName) where T : PlayableAsset
        {
            var tracks = GetTrackList();
            TrackAsset track = _tracks.FirstOrDefault(x => x.name == trackName);
            if (track != null)
            {
                var clips = track.GetClips();
                TimelineClip clip = clips.FirstOrDefault(x => x.displayName == clipName);
                if (clip != null && clip.asset != null)
                {
                    return clip.asset as T;
                }
            }

            return null;
        }

        /// <summary>
        /// TrackListの取得
        /// </summary>
        IEnumerable<TrackAsset> GetTrackList()
        {
            if (_tracks == null)
            {
                _tracks = (director.playableAsset as TimelineAsset).GetOutputTracks();
            }

            return _tracks;
        }
    }
}
