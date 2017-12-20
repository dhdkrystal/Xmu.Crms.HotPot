using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CourseManagementSystem.Controllers
{
    public class CoursesController : ApiController
    {
        private CourseManagementSystemContext db = new CourseManagementSystemContext();

        // GET: Course
        [Route("Course")]
        [HttpGet]
        public List<CourseListInfoModel> GetCourses()
        {
            return new List<CourseListInfoModel> { new CourseListInfoModel { Id=1, Name="OOAD", NumClass=6}, new CourseListInfoModel { Id = 2, Name = "JavaEE", NumClass = 4 }, new CourseListInfoModel { Id = 3, Name = "Network", NumClass = 3 } };
            //using CourseListInfoModel
            //return db.Courses;  
        }

        // GET: Course/5
        [Route("Course")]
        [ResponseType(typeof(Course))]
        public IHttpActionResult GetCourse(int id)
        {
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return NotFound();
            }

            return Ok(new CourseInfoModel { Id = course.Id, Name = course.Name, Description = course.Description});
        }

        // PUT: api/Courses/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCourse(int id, Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != course.Id)
            {
                return BadRequest();
            }

            db.Entry(course).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: Course
        [Route("Course")]
        [ResponseType(typeof(Course))]
        public AddClassResultModel PostCourse(Course course)
        {
            if (!ModelState.IsValid)
            {
                return null;
            }

            db.Courses.Add(course);
            db.SaveChanges();

            return new AddClassResultModel { Id = course.Id };
        }

        // DELETE: Course/5
        [Route("Course")]
        [ResponseType(typeof(Course))]
        public IHttpActionResult DeleteCourse(int id)
        {
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return NotFound();
            }

            db.Courses.Remove(course);
            db.SaveChanges();

            return Ok(course);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CourseExists(int id)
        {
            return db.Courses.Count(e => e.Id == id) > 0;
        }
    }
}