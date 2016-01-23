using ImageGallery.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
//using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ImageGallery.Controllers
{
    //[Authorize]
    public class PhotosController : Controller
    {
        private readonly ApplicationDBContext db;
        private IHostingEnvironment _environment;
        public PhotosController(IHostingEnvironment environment)
        {
            _environment = environment;
            db = new ApplicationDBContext();
        }

        //public PhotosController(ApplicationDBContext dbContext)
        //{
        //    this.db = dbContext;
        //}

        //[HttpGet]
        //[Route("/api/[controller]")]
        //public async Task<IActionResult> AllAsync()
        //{
        //    return Json(await db.Photos.AllAsync()); 
        //}

        public IActionResult Index(Int16? kind, int? kindId)
        {
            //if (kind == null || kindId == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}

            ViewBag.Kind = kind;
            ViewBag.KindId = kindId;
            var items = db.Photos.Where(p => p.Kind == kind && p.KindId == kindId).Select(p => p);
            //foreach (var item in items)
            //{
            //    var localPath = Path.Combine(_environment.WebRootPath, "Temp", item.Id.ToString() + "_thumb.png");
            //    if (!System.IO.File.Exists(localPath))
            //    {
            //        try
            //        {
            //            using (MemoryStream data = new MemoryStream())
            //            {
            //                byte[] buf = item.Content;
            //                data.Write(buf, 0, buf.Length);
            //                data.Seek(0, SeekOrigin.Begin); // <-- missing line

            //                Image image = Image.FromStream(data, false);
            //                Image thumb = image.GetThumbnailImage(100, 100, () => false, IntPtr.Zero);
            //                thumb.Save(localPath, System.Drawing.Imaging.ImageFormat.Png);
            //                thumb.Dispose();
            //            }

            //        }
            //        ///reg add HKEY_CURRENT_USER\Software\Microsoft\VisualStudio\14.0\WebProjects /v Use64BitIISExpress /t REG_DWORD /d 1
            //        catch (OutOfMemoryException)
            //        {
            //            //// Well, this looks like a buggy image. Try using alternate method   
            //            //ImageMagick.MagickImage image = new ImageMagick.MagickImage(imagePath);
            //            //image.Resize(image.Width, image.Height);
            //            //image.Quality = 90;
            //            //image.CompressionMethod = ImageMagick.CompressionMethod.JPEG;
            //            //Image bitmap = image.ToBitmap();
            //            //Image thumb = bitmap.GetThumbnailImage(100, 100, () => false, IntPtr.Zero);
            //            //thumb.Dispose();
            //        }
            //        catch (Exception)
            //        {
            //            //System.Diagnostics.Debug.WriteLine(ex.Message);
            //        }
            //    }
            //}

            return View(items.ToList());
            //return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IList<IFormFile> files, int? id, Int16? kind, int? kindId)
        {
            foreach (var file in files)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"').ToLower();// FileName returns "fileName.ext"(with double quotes) in beta 3
 
                if (fileName.EndsWith(".jpg") 
                    || fileName.EndsWith(".png")
                    || fileName.EndsWith(".gif")
                    || fileName.EndsWith(".bmp")
                    || fileName.EndsWith(".tif")
                    || fileName.EndsWith(".tiff")
                    || fileName.EndsWith(".ico")

                    || fileName.EndsWith(".pdf")
                    )// Important for security if saving in webroot
                {
                    Photo photo = null;
                    if (id.HasValue)
                    {
                        photo = await FindPhotoAsync(id.Value);
                    }
                    else
                    {
                        photo = new Models.Photo();
                        photo.Kind = kind.Value;
                        photo.KindId = kindId.Value;
                    }

                    photo.Name = Path.GetFileName(fileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "uploads", photo.Name) ;
                    using (Stream str = file.OpenReadStream())
                    {
                        //byte[] buf = new byte[file.Length];
                        //str.Read(buf, 0, buf.Length);
                        //photo.Content = buf;

                        using (MemoryStream data = new MemoryStream())
                        {
                            str.CopyTo(data);
                            data.Seek(0, SeekOrigin.Begin); // <-- missing line
                            byte[] buf = new byte[data.Length];
                            data.Read(buf, 0, buf.Length);
                            photo.Content = buf;
                        }
                    }
                    //await file.SaveAsAsync(filePath);
                    //photo.Content = System.IO.File.ReadAllBytes(filePath);
                    db.Photos.Add(photo);
                    await db.SaveChangesAsync();

                    // save the image path path to the database or you can send image
                    // directly to database
                    // in-case if you want to store byte[] ie. for DB
                    //using (MemoryStream ms = new MemoryStream())
                    //{
                    //    file2.InputStream.CopyTo(ms);
                    //    byte[] array = ms.GetBuffer();
                    //}
                }
            }
            return RedirectToAction("Index", new { kind = kind, kindId = kindId });// PRG
            //return View();
        }
        //public byte[] imageToByteArray(System.Drawing.Image imageIn)
        //{
        //    MemoryStream ms = new MemoryStream();
        //    imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
        //    return ms.ToArray();
        //}

        ////Byte array to photo
        //public Image byteArrayToImage(byte[] byteArrayIn)
        //{
        //    MemoryStream ms = new MemoryStream(byteArrayIn);
        //    Image returnImage = Image.FromStream(ms);
        //    return returnImage;
        //}

        //[UserAuthorization(Roles = "ContentAuthor,Administrator")]
        public async Task<ActionResult> Download(int id) //
        {
            Photo photo = await FindPhotoAsync(id);
            if (photo == null)
            {
                //Logger.LogInformation("Edit: Item not found {0}", id);
                return HttpNotFound();
            }
            return File(photo.Content, "application/" + photo.Extension, photo.Name); // Server.UrlEncode(photo.Name)
        }



        //[UserAuthorization(Roles = "ContentAuthor,Administrator")]
        public async Task<ActionResult> Edit(int id)
        {
            Photo photo = await FindPhotoAsync(id);
            if (photo == null)
            {
                //Logger.LogInformation("Edit: Item not found {0}", id);
                return HttpNotFound();
            }

            //ViewBag.MapToId = new SelectList(db.AircraftManufacturers.Select(cn => new { Id = cn.Id, Name = cn.Name }).Distinct().Where(c => !string.IsNullOrEmpty(c.Name)).OrderBy(c => c.Name), "Id", "Name", photo.MapToId);

            //if (Request.IsAjaxRequest())
            //{
            //    return PartialView(photo);
            //}
            return View(photo);
        }

        private Task<Photo> FindPhotoAsync(int id)
        {
            return db.Photos.SingleOrDefaultAsync(p => p.Id == id);
        }

        // POST: AircraftManufacturers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[UserAuthorization(Roles = "ContentAuthor,Administrator")]
        public async Task<ActionResult> Update(int id, [Bind("Id,Kind,KindId,Name,Description,Content")] Photo photo) 
        {
            if (ModelState.IsValid)
            {
                if (photo.Content == null)
                {
                    photo.Content = db.Photos.Where(p => p.Id == id).Select(p => p.Content).FirstOrDefault();
                }
                db.Entry(photo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                //return RedirectToAction("Index");
                //return RedirectToAction("Details", new { id = aircraftManufacturer.Id });
                //return Redirect(this.Request?.UrlReferrer?.AbsoluteUri ?? "~/");
            }

            //ViewBag.MapToId = new SelectList(db.AircraftManufacturers.Select(cn => new { Id = cn.Id, Name = cn.Name }).Distinct().Where(c => !string.IsNullOrEmpty(c.Name)).OrderBy(c => c.Name), "Id", "Name", photo.MapToId);

            //if (this.Request != null && Request.IsAjaxRequest())
            //{
            //    return PartialView(photo);
            //}
            return RedirectToAction("Index", new { kind = photo.Kind, kindId = photo.KindId });// PRG
        }

        //[UserAuthorization(Roles = "ContentAuthor,Administrator")]
        public async Task<ActionResult> Delete(int id)
        {
            Photo photo = await FindPhotoAsync(id);
            if (photo == null)
            {
                //Logger.LogInformation("Edit: Item not found {0}", id);
                return HttpNotFound();
            }

            //if (this.Request != null && this.Request.IsAjaxRequest())
            //{
            //    return PartialView(photo);
            //}
            return View(photo);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[UserAuthorization(Roles = "ContentAuthor,Administrator")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Photo photo = await FindPhotoAsync(id);
            if (photo == null)
            {
                //Logger.LogInformation("Edit: Item not found {0}", id);
                return HttpNotFound();
            }
            db.Photos.Remove(photo);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { kind = photo.Kind, kindId = photo.KindId });// PRG
        }

    }
}
