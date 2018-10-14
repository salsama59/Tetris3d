using UnityEngine;

public class WallColideBehaviour : MonoBehaviour {

    public GameObject sparkEffects;

    private void OnTriggerEnter(Collider other)
    {
        bool isOtherColliderHasPieceChildTag = other.CompareTag(TagConstants.TAG_NAME_PLAYER_1_PIECE_CHILD) || other.CompareTag(TagConstants.TAG_NAME_PLAYER_2_PIECE_CHILD);
        PieceMetadatas pieceMetadatasScript = other.GetComponent<PieceMetadatas>();
        if (isOtherColliderHasPieceChildTag && !pieceMetadatasScript.IsSparkling)
        {

            PieceMovement parentPieceMovementScript = null;
            parentPieceMovementScript = other.GetComponentInParent<PieceMovement>();

            if (parentPieceMovementScript == null)
            {
                return;
            }

            //If the piece is moving
            if (parentPieceMovementScript.IsMoving)
            {
                PieceMetadatas parentPieceMetadatasScript = other.transform.parent.GetComponent<PieceMetadatas>();
                Vector3 sparkPosition = new Vector3(this.transform.position.x, other.transform.position.y, other.transform.position.z);
                GameObject currentSpark = Instantiate(sparkEffects, sparkPosition, sparkEffects.transform.rotation);

                currentSpark.transform.parent = other.gameObject.transform.parent;

                pieceMetadatasScript.IsSparkling = true;
                parentPieceMetadatasScript.IsSparkling = true;
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        PieceMetadatas pieceMetadatasScript = other.GetComponent<PieceMetadatas>();
        if(pieceMetadatasScript.IsSparkling)
        {
            PieceMetadatas parentPieceMetadatasScript = other.GetComponentInParent<PieceMetadatas>();
            parentPieceMetadatasScript.IsSparkling = false;
            GameObject[] effectList = GameObject.FindGameObjectsWithTag(TagConstants.TAG_NAME_SPARKLE_EFFECT);
            foreach (GameObject effect in effectList)
            {
                effect.transform.parent = null;
                Destroy(effect);
            }
            pieceMetadatasScript.IsSparkling = false;
        }
    }

}
