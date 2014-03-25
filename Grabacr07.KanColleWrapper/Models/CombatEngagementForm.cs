using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// �퓬�ɂ�������`�Ԃ��������ʎq���擾���܂��B
	/// </summary>
	public enum CombatEngagementForm
	{
		Unknown,

		/// <summary>
		/// ���q��B
		/// </summary>
		Parallel = 1,

		/// <summary>
		/// ���R��
		/// </summary>
		HeadOn = 2,

		/// <summary>
		/// ������ (�L��)�B
		/// </summary>
		CrossingTAdvantage = 3,

		/// <summary>
		/// ������ (�s��)�B
		/// </summary>
		CrossingTDisadvantage = 4,
	}
}