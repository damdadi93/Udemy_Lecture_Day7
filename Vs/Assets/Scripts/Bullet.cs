using UnityEngine;
using System.Collections;


/*
 *	弾クラス
 *	Maruchu
 *
 *	何でもいいので接触したらエフェクトを出して消える
 */


//총알 클래스
// 무엇이든 이 총알에 닿으면 총알은 효과를 표시하며 사라진다.
public class Bullet : HitObject
{


	//1초 동안
	private static readonly float bulletMoveSpeed = 10.0f;                  //1秒間に弾が進む距離


	public GameObject hitEffectPrefab = null;                       //ヒットエフェクトのプレハブ




	/*
	 *	毎フレーム呼び出される関数
	 */
	private void Update()
	{

		//移動
		{
			//1秒間の移動量
			Vector3 vecAddPos = (Vector3.forward * bulletMoveSpeed);
			/*
					Vector3.forward は new Vector3( 0f, 0f, 1f) と同じです

					他にも色々あるので↓のページを参照してみてください
					http://docs.unity3d.com/ScriptReference/Vector3.html

					そして Vector3 に transform.rotation をかけると、その方向へ曲げてくれます
					この時、Vector3 は Z+ の方向を正面として考えます
			 */

			//移動量、回転量には Time.deltaTime をかけて実行環境(フレーム数の差)による違いが出ないようにします
			transform.position += ((transform.rotation * vecAddPos) * Time.deltaTime);
		}
	}



	/*
	 *	Collider が何かにヒットしたら呼ばれる関数
	 *
	 *	自分の GameObject に Collider(IsTriggerをつける) と Rigidbody をつけると呼ばれるようになります
	 */
	private void OnTriggerEnter(Collider hitCollider)
	{

		//히트(닿았을 때) 검사
		if (false == IsHitOK(hitCollider.gameObject))
		{
			//히트가 없으면 그냥 종료
			return;
		}

		//히트 효과 프리팹이 있으면
		if (null != hitEffectPrefab)
		{
			//현재 위치에 히트 효과 생성
			Instantiate(hitEffectPrefab, transform.position, transform.rotation);
		}

		//해당 게임오브젝트를 Hierachy에서 제거
		Destroy(gameObject);
	}





}
