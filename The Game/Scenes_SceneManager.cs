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
            // _sceneManager ??= new SceneManager(); // C# 8.0+
            if (_sceneManager == null) _sceneManager = new SceneManager();
            return _sceneManager;
        }

    public IScene PrepareScene<T>(SceneArgs instructions) where T : BaseScene, new()
        {
            _scene?.Dispose();
            _scene = new T();
            _scene.Init(instructions);
            return _scene;
        }
    }
}
