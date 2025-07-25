using System;
using System.Collections.Generic;
using GameFramework;
using GameApp.Singleton;

namespace GameApp.Hot.UI
{
    public sealed class ModelManager : Singleton<ModelManager>
    {
        private readonly Dictionary<string, IModel> m_ModelDic;

        public ModelManager()
        {
            m_ModelDic = new Dictionary<string, IModel>();
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (KeyValuePair<string, IModel> _kv in m_ModelDic)
            {
                _kv.Value.Clear();
            }
        }

        public T AddModel<T>() where T : IModel, new()
        {
            string _fullName = typeof(T).FullName;
            if (string.IsNullOrEmpty(_fullName) || m_ModelDic.ContainsKey(_fullName))
            {
                throw new Exception(Utility.Text.Format("Model type '{0}' already exist.", _fullName));
            }

            T _model = new T();
            m_ModelDic.Add(_fullName, _model);
            return _model;
        }

        public IModel AddModel(Type modelType)
        {
            if (modelType == null)
            {
                throw new Exception("Model type is invalid.");
            }

            if (!modelType.IsClass || modelType.IsAbstract)
            {
                throw new Exception("Model type is not a non-abstract class type.");
            }

            if (!typeof(IModel).IsAssignableFrom(modelType))
            {
                throw new Exception(Utility.Text.Format("Model type '{0}' is invalid.", modelType.FullName));
            }

            string _typeName = modelType.FullName;
            if (string.IsNullOrEmpty(_typeName) || m_ModelDic.ContainsKey(_typeName))
            {
                throw new Exception(Utility.Text.Format("Model type '{0}' already exist.", _typeName));
            }

            IModel _model = (IModel)Activator.CreateInstance(modelType);
            m_ModelDic.Add(_typeName, _model);
            return _model;
        }

        public T GetModel<T>() where T : IModel
        {
            return (T)InternalGetModel(typeof(T).FullName);
        }

        public IModel GetModel(Type modelType)
        {
            if (modelType == null)
            {
                throw new Exception("Model type is invalid.");
            }

            if (!modelType.IsClass || modelType.IsAbstract)
            {
                throw new Exception("Model type is not a non-abstract class type.");
            }

            if (!typeof(IModel).IsAssignableFrom(modelType))
            {
                throw new Exception(Utility.Text.Format("Model type '{0}' is invalid.", modelType.FullName));
            }

            return InternalGetModel(modelType.FullName);
        }

        private IModel InternalGetModel(string modelName)
        {
            if (!m_ModelDic.TryGetValue(modelName, out IModel _model))
            {
                throw new Exception(Utility.Text.Format("Model type '{0}' not exist.", modelName));
            }

            return _model;
        }
    }
}