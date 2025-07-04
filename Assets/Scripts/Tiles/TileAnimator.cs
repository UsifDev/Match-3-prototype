using System;
using UnityEngine;

// ENCAPSULATION
namespace Tiles
{
    public class TileAnimator : MonoBehaviour
    {
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
        private Animation currentAnimationStrategy = null;


        void Start()
        {
            IsBeingDestroyed = false;
        }

        void Update()
        {
            if (!IsBusy)
            {
                targetPos = transform.position;
                currentAnimationStrategy = null;
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
            if (IsBusy) return;
            targetPos = other;
            currentAnimationStrategy = new MovingAnimation();
        }

        // POLYMORPHISM
        public void AnimateMovement(int countOfTilesToFall)
        {
            if (IsBusy) return;
            targetPos = new Vector3(transform.position.x, transform.position.y - (countOfTilesToFall * GameManager.GameScaling), 0);
            currentAnimationStrategy = new MovingAnimation();
        }

        public void OnDestroyTile()
        {
            if (IsBusy) return;
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
            float ANIMSPEED = 0.05f;

            public override void Animate(Transform transform, Vector3 targetPos)
            {
                var tile = transform.gameObject;
                var spriteRenderer = tile.GetComponent<SpriteRenderer>();

                if (spriteRenderer == null) 
                { 
                    isBusy = false; 
                    return;
                }

                if (tile.transform.localScale.x < 0.05f)
                {
                    isBusy = false;
                }

                Color startColor = spriteRenderer.color;
                Vector3 startScale = tile.transform.localScale;

                spriteRenderer.color = Color.Lerp(startColor, Color.clear, ANIMSPEED);
                tile.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, ANIMSPEED * GameManager.GameScaling);
            }
        }

        // INHERITANCE
        class MovingAnimation : Animation
        {
            int z = int.MaxValue;
            const float MINRADIUS = 0.05f;
            const float ANIMSPEED = 9f;
            public override void Animate(Transform transform, Vector3 targetPos)
            {
                // since cross product in anti-commutative we can detect when the vector flips
                if (z == float.PositiveInfinity) 
                    z = Math.Sign(Vector3.Cross(transform.position, targetPos).z);
                
                var direction = targetPos - transform.position;
                transform.Translate(Time.deltaTime * ANIMSPEED * direction.normalized * GameManager.GameScaling);

                if (Math.Sign(Vector3.Cross(transform.position, targetPos).z) == z ||
                    transform.position == targetPos ||
                    Vector3.Distance(transform.position, targetPos) <= MINRADIUS * GameManager.GameScaling)
                {
                    transform.position = new Vector3((int)targetPos.x, (int)targetPos.y);
                    isBusy = false;
                }
            }
        }
    }
}
