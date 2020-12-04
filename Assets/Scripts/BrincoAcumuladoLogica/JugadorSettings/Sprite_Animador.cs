using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

namespace Brinco
{
    public class Sprite_Animador : MonoBehaviour
    {
        [Tooltip("idle,corrienda,brinco,caida,muerte")]
        public string animDebug = "brinco";
        public List<Sprite> sprites_idle;
        public List<Sprite> sprites_corriendo;
        public List<Sprite> sprites_brinco;
        public List<Sprite> sprites_caida;
        public List<Sprite> sprites_golpe;

        [SerializeField]
        private List<Sprite> sprites_actuales = new List<Sprite>();
        public int currentFrame;
        public float tiempo;
        public float frameRate = 0.1f;
        private SpriteRenderer spriteRenderer;

        public bool loop = true;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            CambioAnimacion("idle");
        }
        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            Reproductor();
        }

        public void Reproductor()
        {
            if (sprites_actuales.Count <= 0)
                return;

            tiempo += Time.deltaTime;

            if (tiempo >= frameRate)
            {
                tiempo -= frameRate;

                if (currentFrame < sprites_actuales.Count - 1)
                {
                    currentFrame = (currentFrame + 1);

                }
                else if (loop)
                {
                    currentFrame = (currentFrame + 1) % sprites_actuales.Count;//el sobrante resetea automaticamente a 0

                }

                spriteRenderer.sprite = sprites_actuales[currentFrame];
            }

        }
        [Button]
        public void CambioAnimacion()
        {
            CambioAnimacion(animDebug);
            loop = false;
        }
        /// <summary>
        /// Cambia la animacion de acuerdo a lo que indique en tipoAnimacion
        /// </summary>
        /// <param name="tipoAnimacion">idle,corrienda,brinco,caida,muerte</param>
        public void CambioAnimacion(string tipoAnimacion)
        {
            if (tipoAnimacion == "idle")
            {
                // sprites_actuales.Clear();
                sprites_actuales = sprites_idle;
            }
            else if (tipoAnimacion == "corriendo")
            {
                sprites_actuales = sprites_corriendo;

            }
            else if (tipoAnimacion == "brinco")
            {
                sprites_actuales = sprites_brinco;

            }
            else if (tipoAnimacion == "caida")
            {
                sprites_actuales = sprites_caida;

            }
            else if (tipoAnimacion == "muerte")
            {
                sprites_actuales = sprites_golpe;

            }
            else
            {
                Debug.Log("No se encontro la animacion: " + tipoAnimacion + " cambiando a IDLE");
                sprites_actuales = sprites_idle;

            }


            // Debug.Log("se cambio sprite: "+tipoAnimacion);
            currentFrame = 0;


        }

        /// <summary>
        /// Cambia la Animacion y ademas necesita saber el estado del loop,
        /// llama a CambioAnimacion
        /// </summary>
        ///
        /// <seealso cref="CambioAnimacion(string tipoAnimacion)"/>
        /// <param name="tipoAnimacion">idle,corriendo,brinco,caida,muerte</param>
        /// <param name="loopear">true/false</param>
        public void CambioAnimacion(string tipoAnimacion, bool loopear)
        {
            loop = loopear;
            CambioAnimacion(tipoAnimacion);
        }

        public void CambiarSprites(PersonajesEnJuego personaje_sprites)
        {
            sprites_idle = personaje_sprites.personaje.sprites_idle;
            sprites_corriendo = personaje_sprites.personaje.sprites_corriendo;
            sprites_brinco = personaje_sprites.personaje.sprites_brinco;
            sprites_caida = personaje_sprites.personaje.sprites_caida;
            sprites_golpe = personaje_sprites.personaje.sprites_muerte;

            CambioAnimacion("idle");

            Debug.Log("Se cambiaron a sprites de:" + personaje_sprites.personaje.nombre);
        }
    }
}
