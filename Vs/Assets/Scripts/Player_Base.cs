using UnityEngine;
using System.Collections;




/*
 *	プレーヤークラス 基底
 *	Maruchu
 *
 *	キャラクターの移動、メカニム(モーション)の制御など
 */
public class Player_Base : HitObject
{




	//プレーヤーの操作の種類
	protected enum PlayerInput
	{
		Move_Left       //移動 左
		, Move_Up       //移動 上
		, Move_Right        //移動 右
		, Move_Down     //移動 下
		, Shoot         //射撃
		, EnumMax       //最大数
	}

	private static readonly float MOVE_ROTATION_Y_LEFT = -90f;      //移動方向 左
	private static readonly float MOVE_ROTATION_Y_UP = 0f;      //移動方向 上
	private static readonly float MOVE_ROTATION_Y_RIGHT = 90f;      //移動方向 右
	private static readonly float MOVE_ROTATION_Y_DOWN = 180f;      //移動方向 下

	public float MOVE_SPEED = 5.0f;     //移動の速度





	public GameObject playerObject = null;      //動かす対象のモデル
	public GameObject bulletObject = null;      //弾プレハブ


	public GameObject hitEffectPrefab = null;       //ヒットエフェクトのプレハブ





	private float m_rotationY = 0.0f;       //플레이어 회전 각도

	protected bool[] m_playerInput = new bool[(int)PlayerInput.EnumMax];        //키를 누르고

	protected bool m_playerDeadFlag = false;        //플레이어 사망 여부를 정하는 플래그

    //m_멤버변수 표현




    /*
	 *	毎フレーム呼び出される関数
	 */

    // 각 프레임에서 호출되는 함수
    private void Update()
	{

		// 플레이어가 사망한 상태
		if (m_playerDeadFlag)
		{
			//모든 처리를 무시한다.
			return;
		}

		//フラグ初期化
		ClearInput();
		//入力処理取得
		GetInput();

		//移動処理
		CheckMove();
	}


	/*
	 *	入力処理のチェック
	 */

	//입력 처리 초기화
	private void ClearInput()
	{
		//플래그
		int i;
		for (i = 0; i < (int)PlayerInput.EnumMax; i++)
		{
			m_playerInput[i] = false;
		}
	}
	/*
	 *	入力処理のチェック
	 */
	protected virtual void GetInput()
	{
	}


	/*
	 *	移動処理のチェック
	 */

	//이동처리 검사
	private void CheckMove()
	{

		//アニメーター(メカニム)を取得
		Animator animator = playerObject.GetComponent<Animator>();

		//弾にあたってなければ移動OK
		float moveSpeed = MOVE_SPEED;       //移動速度
		bool shootFlag = false;         //弾を撃つフラグ

		//이동과 회전
		{
			//키 조작으로 회전과 이동
			if (m_playerInput[(int)PlayerInput.Move_Left])
			{
				//左
				m_rotationY = MOVE_ROTATION_Y_LEFT;
			}
			else
			if (m_playerInput[(int)PlayerInput.Move_Up])
			{
				//上
				m_rotationY = MOVE_ROTATION_Y_UP;
			}
			else
			if (m_playerInput[(int)PlayerInput.Move_Right])
			{
				//右
				m_rotationY = MOVE_ROTATION_Y_RIGHT;
			}
			else
			if (m_playerInput[(int)PlayerInput.Move_Down])
			{
				//下
				m_rotationY = MOVE_ROTATION_Y_DOWN;
			}
			else
			{
				//아무것도 누르지 않으면
				moveSpeed = 0f;
			}

			//向いている方向をオイラー角で入れる
			transform.rotation = Quaternion.Euler(0, m_rotationY, 0);       //Y軸回転でキャラの向きを横に動かせます

			//移動量を Transform に渡して移動させる
			transform.position += ((transform.rotation * (Vector3.forward * moveSpeed)) * Time.deltaTime);
		}

		//射撃
		{
			//射撃ボタン(クリック)押してる？
			if (m_playerInput[(int)PlayerInput.Shoot])
			{
				//撃った
				shootFlag = true;

				//弾を生成する位置
				Vector3 vecBulletPos = transform.position;
				//進行方向にちょっと前へ
				vecBulletPos += (transform.rotation * Vector3.forward);
				//Yは高さを適当に上げる
				vecBulletPos.y = 2.0f;

				//총알을 생성한다.
				Instantiate(bulletObject, vecBulletPos, transform.rotation);
			}
			else
			{
				//쏘지 않는다.
				shootFlag = false;
			}
		}


		//メカニム
		{
			//Animatorで設定した値を渡す
			animator.SetFloat("Speed", moveSpeed);      //移動量
			animator.SetBool("Shoot", shootFlag);       //射撃フラグ
		}
	}




	/*
	 *	Collider が何かにヒットしたら呼ばれる関数
	 *
	 *	自分の GameObject に Collider(IsTriggerをつける) と Rigidbody をつけると呼ばれるようになります
	 */
	private void OnTriggerEnter(Collider hitCollider)
	{

		//ヒットして良いか確認
		if (false == IsHitOK(hitCollider.gameObject))
		{
			//このオブジェクトにはあたってはいけない
			return;
		}

		//弾に当たった
		{
			//アニメーター(メカニム)を取得
			Animator animator = playerObject.GetComponent<Animator>();

			//メカニムに死んだことを通知
			animator.SetBool("Dead", true);     //死んだフラグ
		}

		//ヒットエフェクトある？
		if (null != hitEffectPrefab)
		{
			//自分と同じ位置でヒットエフェクトを出す
			Instantiate(hitEffectPrefab, transform.position, transform.rotation);
		}

		//このプレーヤーは死んだ状態にする
		m_playerDeadFlag = true;
	}




}
