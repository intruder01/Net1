﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

using IList = System.Collections.IList;

using AdamsLair.WinForms.Drawing;
using AdamsLair.WinForms.PropertyEditing.Templates;

namespace AdamsLair.WinForms.PropertyEditing.Editors
{
	public class IListPropertyEditor : GroupedPropertyEditor
	{
		public delegate void IndexValueSetter(PropertyInfo indexer, IEnumerable<object> targetObjects, IEnumerable<object> values, int index);

		private	bool					buttonIsCreate	= false;
		private	NumericPropertyEditor	sizeEditor		= null;
		private	NumericPropertyEditor	offsetEditor	= null;
		private	int						offset			= 0;
		private	int						internalEditors	= 0;
		private	IndexValueSetter		listIndexSetter	= null;
		
		public override object DisplayedValue
		{
			get { return this.GetValue(); }
		}
		public IndexValueSetter ListIndexSetter
		{
			get { return this.listIndexSetter; }
			set
			{
				if (value == null) value = DefaultPropertySetter;
				this.listIndexSetter = value;
			}
		}

		public IListPropertyEditor()
		{
			this.Hints |= HintFlags.HasButton | HintFlags.ButtonEnabled;

			this.listIndexSetter = DefaultPropertySetter;

			this.sizeEditor = new NumericPropertyEditor();
			this.sizeEditor.EditedType = typeof(int);
			this.sizeEditor.Minimum = 0;
			this.sizeEditor.PropertyName = "Size";
			this.sizeEditor.Getter = this.SizeValueGetter;
			this.sizeEditor.Setter = this.SizeValueSetter;

			this.offsetEditor = new NumericPropertyEditor();
			this.offsetEditor.EditedType = typeof(uint);
			this.offsetEditor.Minimum = 0;
			this.offsetEditor.PropertyName = "Offset";
			this.offsetEditor.Getter = this.OffsetValueGetter;
			this.offsetEditor.Setter = this.OffsetValueSetter;
			this.offsetEditor.ValueMutable = true;
		}

		public override void InitContent()
		{
			base.InitContent();

			if (this.EditedType != null)
				this.PerformGetValue();
			else
				this.ClearContent();
		}
		public override void ClearContent()
		{
			base.ClearContent();
			this.offset = 0;
		}

		protected override void OnGetValue()
		{
			base.OnGetValue();
			IList[] values = this.GetValue().Cast<IList>().ToArray();

			string valString = null;
			if (!values.Any() || values.All(o => o == null))
			{
				this.ClearContent();

				this.Hints &= ~HintFlags.ExpandEnabled;
				this.ButtonIcon = AdamsLair.WinForms.Properties.ResourcesCache.ImageAdd;
				this.buttonIsCreate = true;
				this.Expanded = false;
					
				valString = "null";
			}
			else
			{
				if (this.ContentInitialized)
				{
					if (this.Expanded)
						this.UpdateElementEditors(values);
					else
						this.ClearContent();
				}
				
				this.Hints |= HintFlags.ExpandEnabled;
				if (!this.CanExpand) this.Expanded = false;
				this.ButtonIcon = AdamsLair.WinForms.Properties.ResourcesCache.ImageDelete;
				this.buttonIsCreate = false;

				valString = values.Count() == 1 ? 
					string.Format("{0}, Count = {1}", this.EditedType.GetTypeCSCodeName(true), values.First().Count) :
					string.Format(AdamsLair.WinForms.Properties.Resources.PropertyGrid_N_Objects, values.Count());
			}

			this.HeaderValueText = valString;

			foreach (PropertyEditor e in this.Children)
				e.PerformGetValue();
		}
		protected override void OnSetValue()
		{
			if (this.ReadOnly) return;
			if (!this.Children.Any()) return;
			base.OnSetValue();

			foreach (PropertyEditor e in this.Children)
				e.PerformSetValue();
		}
		protected override void VerifyReflectedTypeEditors(IEnumerable<object> values)
		{
			base.VerifyReflectedTypeEditors(values);
			if (!this.ContentInitialized) return;
			if (!this.Expanded) return;

			IList[] valuesCast = values.Cast<IList>().ToArray();
			if (values.Any() && values.Any(o => o != null))
			{
				this.UpdateElementEditors(valuesCast);
			}
		}

		protected void UpdateElementEditors(IList[] values)
		{
			PropertyInfo indexer = typeof(IList).GetProperty("Item");
			IEnumerable<IList> valuesNotNull = values.Where(v => v != null);
			int visibleElementCount = valuesNotNull.Min(o => (int)o.Count);
			bool showOffset = false;
			if (visibleElementCount > 100) //test only
			{
				this.offset = Math.Min(this.offset, visibleElementCount - 100);
				this.offsetEditor.Maximum = visibleElementCount - 100;
				this.offsetEditor.ValueBarMaximum = this.offsetEditor.Maximum;
				visibleElementCount = 100;
				showOffset = true;
			}
			else
			{
				this.offset = 0;
			}

			if (this.sizeEditor.ParentEditor == null) this.AddPropertyEditor(this.sizeEditor, 0);
			if (showOffset && this.offsetEditor.ParentEditor == null) this.AddPropertyEditor(this.offsetEditor, 1);
			else if (!showOffset && this.offsetEditor.ParentEditor != null) this.RemovePropertyEditor(this.offsetEditor);

			this.internalEditors = showOffset ? 2 : 1;

			this.BeginUpdate();

			// Add missing editors
			Type elementType = GetIListElementType(this.EditedType);
			Type reflectedArrayType = PropertyEditor.ReflectDynamicType(elementType, valuesNotNull.Select(a => GetIListElementType(a.GetType())));
			for (int i = this.internalEditors; i < visibleElementCount + this.internalEditors; i++)
			{
				int elementIndex = i - this.internalEditors + this.offset;
				Type reflectedElementType = PropertyEditor.ReflectDynamicType(
					reflectedArrayType, 
					valuesNotNull.Select(v => indexer.GetValue(v, new object[] { elementIndex })));
				PropertyEditor elementEditor;

				// Retrieve and Update existing editor
				if (i < this.Children.Count())
				{
					elementEditor = this.Children.ElementAt(i);
					if (elementEditor.EditedType != reflectedElementType)
					{
						// If the editor has the wrong type, we'll need to create a new one
						PropertyEditor oldEditor = elementEditor;
						elementEditor = this.ParentGrid.CreateEditor(reflectedElementType, this);
						
						this.AddPropertyEditor(elementEditor, oldEditor);
						this.RemovePropertyEditor(oldEditor);

						this.ParentGrid.ConfigureEditor(elementEditor);
					}
				}
				// Create a new editor
				else
				{
					elementEditor = this.ParentGrid.CreateEditor(reflectedElementType, this);
					this.AddPropertyEditor(elementEditor);
					this.ParentGrid.ConfigureEditor(elementEditor);
				}
				elementEditor.Getter = this.CreateElementValueGetter(indexer, elementIndex);
				elementEditor.Setter = this.CreateElementValueSetter(indexer, elementIndex);
				elementEditor.PropertyName = "[" + elementIndex + "]";
			}
			// Remove overflowing editors
			for (int i = this.Children.Count() - (this.internalEditors + 1); i >= visibleElementCount; i--)
			{
				PropertyEditor child = this.Children.Last();
				this.RemovePropertyEditor(child);
			}
			this.EndUpdate();
		}

		protected IEnumerable<object> SizeValueGetter()
		{
			return this.GetValue().Select(o => o != null ? (object)((IList)o).Count : null);
		}
		protected void SizeValueSetter(IEnumerable<object> values)
		{
			IEnumerator<object> valuesEnum = values.GetEnumerator();
			IList[] targetArray = this.GetValue().Cast<IList>().ToArray();
			Type elementType = GetIListElementType(this.EditedType);
			Type reflectedArrayType = PropertyEditor.ReflectDynamicType(elementType, targetArray.Select(a => GetIListElementType(a.GetType())));

			bool writeBack = false;
			int curValue = 0;
			if (valuesEnum.MoveNext()) curValue = (int)valuesEnum.Current;
			for (int t = 0; t < targetArray.Length; t++)
			{
				IList target = targetArray[t];
				if (target != null)
				{
					if (!target.IsFixedSize && !target.IsReadOnly)
					{
						// Dynamically adjust IList length
						while (target.Count < curValue)
							target.Add(elementType.IsValueType ? reflectedArrayType.CreateInstanceOf() : null);
						while (target.Count > curValue)
							target.RemoveAt(target.Count - 1);
					}
					else if (target is Array)
					{
						// Create new array that replaces the old one
						Array newTarget = Array.CreateInstance(reflectedArrayType, curValue);
						for (int i = 0; i < Math.Min(curValue, target.Count); i++) newTarget.SetValue(target[i], i);
						targetArray[t] = newTarget;
						writeBack = true;
					}
					else
					{
						// Just some read-only container? Well, can't do anything here.
					}
				}
				if (valuesEnum.MoveNext()) curValue = (int)valuesEnum.Current;
			}
			if (writeBack || this.ForceWriteBack) this.SetValues(targetArray);
			this.PerformGetValue();
		}
		protected IEnumerable<object> OffsetValueGetter()
		{
			yield return (uint)this.offset;
		}
		protected void OffsetValueSetter(IEnumerable<object> values)
		{
			this.offset = (int)Convert.ChangeType(values.First(), typeof(int));
			this.PerformGetValue();
		}
		protected Func<IEnumerable<object>> CreateElementValueGetter(PropertyInfo indexer, int index)
		{
			return () => this.GetValue().Select(o => o != null ? indexer.GetValue(o, new object[] {index}) : null);
		}
		protected Action<IEnumerable<object>> CreateElementValueSetter(PropertyInfo indexer, int index)
		{
			return delegate(IEnumerable<object> values)
			{
				object[] targetArray = this.GetValue().ToArray();
				this.listIndexSetter(indexer, targetArray, values, index);
				if (this.ForceWriteBack) this.SetValues(targetArray);
			};
		}

		protected override void OnEditedTypeChanged()
		{
			base.OnEditedTypeChanged();
			this.Expanded = false;
			this.ClearContent();
		}
		protected override void OnButtonPressed()
		{
			base.OnButtonPressed();

			if (this.buttonIsCreate)
			{
				bool anyCreated = false;

				int objectsToCreate = this.GetValue().Count();
				IList[] createdObjects = new IList[objectsToCreate];
				for (int i = 0; i < createdObjects.Length; i++)
				{
					createdObjects[i] = this.ParentGrid.CreateObjectInstance(this.EditedType) as IList;
					if (createdObjects[i] == null)
					{
						Type elementType = GetIListElementType(this.EditedType);
						Type listType = elementType != null ? elementType.MakeArrayType() : null;
						if (listType != null && this.EditedType.IsAssignableFrom(listType))
						{
							createdObjects[i] = this.ParentGrid.CreateObjectInstance(listType) as IList;
						}
					}
					if (createdObjects[i] != null)
					{
						anyCreated = true;
					}
				}

				if (anyCreated)
				{
					this.SetValues(createdObjects);
					this.Expanded = true;
				}
			}
			else
			{
				this.SetValue(null);
			}

			this.PerformGetValue();
		}
		protected internal override void ConfigureEditor(object configureData)
		{
			base.ConfigureEditor(configureData);
		}

		protected static Type GetIListElementType(Type listType)
		{
			Type ilistInterface = null;
			if (listType.HasElementType)
				return listType.GetElementType();
			else if ((ilistInterface = listType.GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>)).FirstOrDefault()) != null)
				return ilistInterface.GetGenericArguments()[0];
			else if (listType.IsGenericType)
				return listType.GetGenericArguments()[0];
			else
				return typeof(object);
		}
		protected static void DefaultPropertySetter(PropertyInfo property, IEnumerable<object> targetObjects, IEnumerable<object> values, int index)
		{
			IEnumerator<object> valuesEnum = values.GetEnumerator();
			object curValue = null;

			if (valuesEnum.MoveNext()) curValue = valuesEnum.Current;
			foreach (object target in targetObjects)
			{
				if (target != null) property.SetValue(target, curValue, new object[] { index });
				if (valuesEnum.MoveNext()) curValue = valuesEnum.Current;
			}
		}
	}
}
