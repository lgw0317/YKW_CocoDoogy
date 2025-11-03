using UnityEngine;
namespace Water
{
    public class Flow : MonoBehaviour
    {
        private Material waterMat;
        [SerializeField] float flowTime = 10f;
        

        public float GetFlowTime()
        {
            return flowTime;
        }

        void Awake()
        {
            waterMat = GetComponent<MeshRenderer>().material;
        }


        public void SetFlowDirection()
        {
            waterMat.SetVector("_FlowDir", new(transform.parent.rotation.x, transform.parent.rotation.y, transform.parent.rotation.z, transform.parent.rotation.w));

        }

        
    }
}