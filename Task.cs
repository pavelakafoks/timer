﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace timer {
	public class Task {
		//private string project;
		public string Project {
			get; set;
		}
		public string Description {
			get; set;
		}

		public enum States { NEW, IN_PROGRESS, PAUSED, FINISHED };
		private States state;
		public States State {
			get { return this.state; }
		}

		private List<WorkTime> workTimes = new List<WorkTime>();

		private TimeSpan finishedDuration;

		public TimeSpan Duration {
			get { return this.finishedDuration + ((this.State == States.FINISHED || this.workTimes.Count == 0) ? new TimeSpan() : DateTime.Now - this.workTimes[0].StartedAt); }
		}

		private TimeSpan expectedTime;

		public struct SerializedForm {
			public string Project;
			public string Description;
			public string ExpectedTime;
			public WorkTime.SerializedForm[] WorkTimes;
		}

		public class WorkTime {
			public DateTime StartedAt;
			public DateTime? FinishedAt;

			public struct SerializedForm {
				public string StartedAt;
				public string FinishedAt;
			}

			public WorkTime() {
				this.StartedAt = DateTime.Now;
				this.FinishedAt = null;
			}

			public SerializedForm Serialize() {
				return new SerializedForm {
					StartedAt = this.StartedAt.ToString(),
					FinishedAt = this.FinishedAt.Value.ToString(),
				};
			}
		}

		public Task(string project, string desciption, TimeSpan expectedTime) {
			// Used to create a new task
			this.state = States.NEW;
			this.Project = project;
			this.Description = desciption;
			this.expectedTime = expectedTime;
		}

		public Task(string json) {
			//JsonReader reader = new JsonReader(json);
		}

		public void Start() {
			if (this.state != States.NEW)
				throw new Exception("Can't start a task that isn't NEW");
			this.state = States.IN_PROGRESS;
			this.workTimes.Insert(0, new WorkTime());
		}

		public void Finish() {
			this.state = States.FINISHED;
			this.workTimes[0].FinishedAt = DateTime.Now;
			this.finishedDuration += DateTime.Now - this.workTimes[0].StartedAt;
		}

		public void Pause() {
			this.state = States.PAUSED;
			this.workTimes[0].FinishedAt = DateTime.Now;
			this.finishedDuration += DateTime.Now - this.workTimes[0].StartedAt;
		}

		public void Resume() {
			this.state = States.IN_PROGRESS;
			this.workTimes.Insert(0, new WorkTime());
		}

		public SerializedForm Serialize() {
			SerializedForm serializedForm = new SerializedForm();
			serializedForm.Project = this.Project;
			serializedForm.Description = this.Description;
			serializedForm.ExpectedTime = this.expectedTime.ToString("hh':'mm");
			List<WorkTime.SerializedForm> workTimes = new List<WorkTime.SerializedForm>();
			foreach (WorkTime workTime in this.workTimes) {
				workTimes.Add(workTime.Serialize());
			}
			serializedForm.WorkTimes = workTimes.ToArray();
			return serializedForm;
		}
	}
}
