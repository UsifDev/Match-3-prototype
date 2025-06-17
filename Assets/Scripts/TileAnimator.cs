using System;
using UnityEngine;

// ENCAPSULATION
namespace Tiles
{
    public class TileAnimator : MonoBehaviour
    {
        [Range(0f,1f)]
        private const float ANIMSPEED = 0.5f; // ENCAPSULATION

        // ENCAPSULATION
        public bool IsBusy { 
            get {
                if (currentAnimationStrategy == null) 
                    return false;
                else 
                    return currentAnimationStrategy.isBusy;
            } 
        } 
        public bool IsBeingDestroyed { get; private set; } // ENCAPSULATION

        private Vector3 targetPos;
        private Animation currentAnimationStrategy;

        void Start()
        {
            targetPos = transform.position;
            IsBeingDestroyed = false;
            currentAnimationStrategy = null;
        }

        void Update()
        {
            if (!IsBusy)
            {
                if (IsBeingDestroyed) 
                    gameObject.SendMessage("DestroyTile");
                return;
            }
            currentAnimationStrategy.Animate(transform, targetPos); // ABSTRACTION
        }

        // ABSTRACTION
        // POLYMORPHISM
        public void AnimateMovement(Vector3 other)
        {
            if (targetPos != transform.position) return;
            targetPos = other;
            currentAnimationStrategy = new MovingAnimation();
        }

        // POLYMORPHISM
        public void AnimateMovement(int countOfTilesToFall)
        {
            if (targetPos != transform.position) return;
            targetPos = new Vector3(transform.position.x, transform.position.y - countOfTilesToFall, 0);
            currentAnimationStrategy = new MovingAnimation();
        }

        public void OnDestroyTile()
        {
            IsBeingDestroyed = true;
            currentAnimationStrategy = new DeathAnimation();
        }

        // INHERITANCE
        abstract class Animation
        {
            public bool isBusy = true;
            public abstract void Animate(Transform transform, Vector3 targetPos);
        }

        // INHERITANCE
        class DeathAnimation : Animation
        { 
            public override void Animate(Transform transform, Vector3 targetPos)
            {
                var tile = transform.gameObject;
                var spriteRenderer = tile.GetComponent<SpriteRenderer>();

                if (spriteRenderer == null) 
                { 
                    isBusy = false; 
                    return;
                }

                if (tile.transform.localScale.x > 0.01f)
                {
                    isBusy = false;
                }

                Color startColor = spriteRenderer.color;
                Vector3 startScale = tile.transform.localScale;

                spriteRenderer.color = Color.Lerp(startColor, Color.clear, ANIMSPEED);
                tile.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, ANIMSPEED);
            }
        }

        // INHERITANCE
        class MovingAnimation : Animation
        {
            public override void Animate(Transform transform, Vector3 targetPos)
            {
                if (targetPos != transform.position)
                    transform.Translate(Time.deltaTime * ANIMSPEED * targetPos.normalized);
                else 
                    isBusy = false;
            }
        }
    }
}
