using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformRotateY : MonoBehaviour {
	public enum RotateType : int {
		CW,
		// Clockwise 顺时针
		CCW,
		// Counterclockwise 逆时针
	}

	public RotateType _type = RotateType.CW;

	public float _srcY = 0;
	public float _desY = 0;

	public enum RotateState : int {
		Stop,
		Start,
		Running,
		End,
	}

	public RotateState _state = RotateState.Stop;

	public float _duration = 0;

	public float _startRange = 0.2f;
	public float _runRange = 0.4f;
	public float _endRange = 0.4f;

	public float _tweenLen = 360 * 10;

	// other debug fields
	float _startDuration = 0;
	float _runDuration = 0;
	// float _endDuration = 0;

	float _wholeLen = 0;

	float _speedUp = 0;
	float _speedDown = 0;

	float _startLen = 0;
	float _runLen = 0;
	// float _endLen = 0;

	float _timePassed = 0;

	// Update is called once per frame
	void Update () {
		switch (_state) {
			case RotateState.Start:
				{
					_timePassed += Time.deltaTime;
					if (_timePassed >= _startDuration) {
						_state = RotateState.Running;
					}
					var len = _srcY + (_speedUp * _timePassed * _timePassed) / 2.0f;
					//					Debug.Log (_state + " - " + len);
					this.transform.localRotation = Quaternion.Euler (new Vector3 (0, len, 0));
				}
				break;
			case RotateState.Running:
				{
					_timePassed += Time.deltaTime;
					if (_timePassed >= _startDuration + _runDuration) {
						_state = RotateState.End;
					}
					float delta = _timePassed - _startDuration;
					float v = _startDuration * _speedUp;
					var len = _srcY + _startLen + (v * delta);
					//					Debug.Log (_state + " - " + len);
					this.transform.localRotation = Quaternion.Euler (new Vector3 (0, len, 0));
				}
				break;
			case RotateState.End:
				{
					_timePassed += Time.deltaTime;
					if (_timePassed >= _duration) {
						_timePassed = _duration;
						this.transform.localRotation = Quaternion.Euler (new Vector3 (0, _desY, 0));
						_state = RotateState.Stop;
					} else {
						float delta = _timePassed - (_startDuration + _runDuration);
						float v = _startDuration * _speedUp;
						var len = _srcY + _startLen + _runLen + (v * delta) + (_speedDown * delta * delta / 2.0f);
						//						Debug.Log (_state + " - " + len);
						this.transform.localRotation = Quaternion.Euler (new Vector3 (0, len, 0));
					}
				}
				break;
			default:
				break;
		}
	}

	public void runRotateTo (float desY_, float duration_) {
		if (_state != RotateState.Stop) {
			Debug.Log ("TransformRotateY runRotate onState" + _state);
			return;
		}

		_desY = desY_;
		_duration = duration_;

		switch (_type) {
			case RotateType.CW:
				{
					_srcY = this.transform.localRotation.eulerAngles.y;

					_wholeLen = _desY - _srcY;
					if (_wholeLen < 0) {
						_wholeLen += 360;
					}
					_wholeLen += _tweenLen;
				}
				break;
			case RotateType.CCW:
				{
					_srcY = this.transform.localRotation.eulerAngles.y;

					_wholeLen = _desY - _srcY;
					if (_wholeLen > 0) {
						_wholeLen -= 360;
					}
					_wholeLen -= _tweenLen;
				}
				break;
		}

		_startDuration = _duration * _startRange;
		_runDuration = _duration * _runRange;
		// _endDuration = _duration * _endRange;

		// _wholeLen * _startRange = (1 / 2) * _speedUp * _startDuration * _startDuration
		// runSpeed = _speedUp * _startDuration
		// _wholeLen * _runRange = runSpeed * _runDuration
		// _wholeLen * _endRange = (1 / 2) * _speedUp * _endDuration * _endDuration
		// _duration = _startDuration + _runDuration + _endDuration

		// (1 / 2) * _speedUp * _startRang * (1 + _runRange) * _duration * _duration = _wholeLen

		_speedUp = (_wholeLen * 2.0f) / (_startRange * (1.0f + _runRange) * _duration * _duration);
		_speedDown = -_speedUp * _startRange / _endRange;

		_startLen = _startDuration * _startDuration * _speedUp / 2.0f;
		//			Debug.Log ("_startLen " + _startLen);
		_runLen = _startDuration * _speedUp * _runDuration;
		//			Debug.Log ("_runLen " + _runLen);
		// _endLen = (_startDuration * _speedUp * _endDuration) + (_speedDown * _endDuration * _endDuration / 2.0f);
		//			Debug.Log ("_endLen " + _endLen);

		_timePassed = 0;
		_state = RotateState.Start;
	}
}