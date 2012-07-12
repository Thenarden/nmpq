using System;
using System.IO;
using NUnit.Framework;

namespace Nmpq.Tests {
	[TestFixture]
	public class StreamTests {
		[Test]
		public void Can_open_stream_for_existing_file() {
			using (var archive = ObjectMother.OpenTestArchive1()) {
				Assert.IsNotNull(archive.OpenFile("(listfile)"));
			}
		}

		[Test]
		public void Null_returned_for_nonexistant_file() {
			using (var archive = ObjectMother.OpenTestArchive1()) {
				Assert.Null(archive.OpenFile("a nonexistant file"));
			}
		}

		[Test]
		public void Can_get_length_for_uncompressed_file() {
			using (var archive = ObjectMother.OpenTestArchive1()) {
				using (var file = archive.OpenFile("replay.message.events")) {
					Assert.That(file.Length, Is.EqualTo(65));
				}
			}
		}

		[Test]
		public void Cannot_have_multiple_files_open_at_once() {
			using (var archive = ObjectMother.OpenTestArchive1()) {
				using (var file = archive.OpenFile("replay.message.events")) {
					Assert.Throws<InvalidOperationException>(() => archive.OpenFile("(listfile)"));
				}
			}
		}

		[Test]
		public void Can_open_another_file_after_closing_first() {
			using (var archive = ObjectMother.OpenTestArchive1()) {
				using (var file = archive.OpenFile("replay.message.events")) {
				}

				Assert.DoesNotThrow(() => archive.OpenFile("replay.message.events"));
			}
		}

		[Test]
		public void Can_read_data_from_uncompressed_file() {
			var expectedBytes = TestUtil.ParseBytes(
					@"00 22 80 00 00 0B 01 00 22 80 00 00 0F 00 00 22 80 00 00 13 00 00 
					  22 80 00 00 17 01 00 22 80 00 00 1B 01 00 22 80 00 00 1E 00 00 22 
					  80 00 00 32 00 01 B2 02 00 04 67 6C 68 66 01 CE 01 00 02 75 32");

			using (var archive = ObjectMother.OpenTestArchive1()) {
				using (var file = archive.OpenFile("replay.message.events")) {
					var bytes = new byte[file.Length];
					var read = file.Read(bytes, 0, bytes.Length);

					Assert.That(read, Is.EqualTo(file.Length));
					Assert.That(bytes, Is.EqualTo(expectedBytes));
				}
			}
		}

		[Test]
		public void Cannot_seek_past_beginning_of_file() {
			using (var archive = ObjectMother.OpenTestArchive1()) {
				using (var file = archive.OpenFile("replay.message.events")) {
					Assert.Throws<InvalidOperationException>(() => file.Seek(-1, SeekOrigin.Begin));
				}
			}
		}

		[Test]
		public void Cannot_seek_past_end_of_file() {
			using (var archive = ObjectMother.OpenTestArchive1()) {
				using (var file = archive.OpenFile("replay.message.events")) {
					Assert.Throws<InvalidOperationException>(() => file.Seek(file.Length + 1, SeekOrigin.Begin));
				}
			}
		}
	}
}