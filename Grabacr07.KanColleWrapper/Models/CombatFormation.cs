using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// �퓬�ɂ�����͑��w�`���������ʎq���擾���܂��B
	/// </summary>
	public enum CombatFormation
	{
		Unknown,

		/// <summary>
		/// �P�c�w�B
		/// </summary>
		LineAhead = 1,

		/// <summary>
		/// ���c�w�B
		/// </summary>
		DoubleLine = 2,

		/// <summary>
		/// �֌`�w�B
		/// </summary>
		Diamond = 3,

		/// <summary>
		/// ��`�w�B
		/// </summary>
		Echelon = 4,

		/// <summary>
		/// �P���w�B
		/// </summary>
		LineAbreast = 5,
	}
}