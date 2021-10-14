using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace The_Game
{
    class SceneManager
    {
        private static SceneManager _sceneManager;
        private BaseScene _scene;

        public static SceneManager Boot()
        {
            if (_sceneManager == null) _sceneManager = new SceneManager();
            return _sceneManager;
        }

        public IScene PrepareScene<T>(Form form) where T : BaseScene, new()
        {
            if (_scene != null) _scene.Dispose();
            _scene = new T();
            _scene.Init(form);
            return _scene;
        }
    }
}
