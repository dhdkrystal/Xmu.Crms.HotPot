using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Services.Group1_7
{
    public class SchoolService : ISchoolService
    {
        private CrmsContext _db;

        // 在构造函数里添加依赖的Service（参考模块标准组的类图）
        public SchoolService(CrmsContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 添加学校.
        /// @author LiuAiqi
        /// </summary>
        /// <param name="school">学校的信息</param>
        /// <returns>schoolId 学校的id</returns>
        public long InsertSchool(School school)
        {

            if (school == null)
            {
                throw new NotImplementedException();
            }
            _db.School.Add(school);
            _db.SaveChanges();
            return school.Id;
        }

        /// <summary>
        /// 获取省份列表.
        /// @author LiuAiqi
        /// </summary>
        /// <returns>list 省份名称列表</returns>
        public IList<string> ListProvince()
        {
            IList<string> provinces = new List<string>();
            IList<School> schools = _db.School.ToList();
            foreach (School school in schools)
                provinces.Add(school.Province);
            IList<string> provinces0 = provinces.Distinct().ToList();
            if (provinces0 == null)
            {
                throw new NotImplementedException();
            }
            return provinces0;
        }

        /// <summary>
        /// 获取城市列表.
        /// @author LiuAiqi
        /// </summary>
        /// <param name="province">省份名称</param>
        /// <returns>list 城市名称列表</returns>
        public IList<string> ListCity(string province)
        {
            IList<string> citys = new List<string>();
            IList<School> schools = _db.School.Where(c => c.Province == province).ToList();
            //ListProvince();
            //IList<string> provinces = new List<string>();

            foreach (var pro in schools)
                citys.Add(pro.City);
            IList<string> citys0 = citys.Distinct().ToList();
            if (citys0 == null)
            {
                throw new NotImplementedException();
            }
            return citys0;
        }

        /// <summary>
        /// 按城市名称查学校.
        /// @author LiuAiqi
        /// </summary>
        /// <param name="city">城市名称</param>
        /// <returns>list 学校列表</returns>
        public IList<School> ListSchoolByCity(string city)
        {
            IList<School> temp = _db.School.ToList();
            IList<School> schools = new List<School>();
            foreach (School school in temp)
            {
                if (school.City == city)
                    schools.Add(school);
            }
            if (schools == null)
            {
                throw new NotImplementedException();
            }
            return schools;
        }

        /// <summary>
        /// 获取学校信息.
        /// @author LiuAiqi
        /// </summary>
        /// <param name="schoolId">学校id</param>
        /// <returns>SchoolBO 学校信息</returns>
        public School GetSchoolBySchoolId(long schoolId)
        {
            var school = _db.School.SingleOrDefault(temp => temp.Id == schoolId);
            if (school == null)
            {
                throw new NotImplementedException();
            }
            return school;
        }
    }
}
