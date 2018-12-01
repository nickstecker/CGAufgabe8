using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static System.Math;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Tutorial.Core
{
    public class FirstSteps : RenderCanvas
    {
       private SceneContainer _scene;
       private SceneRenderer _sceneRenderer;
       private float _camAngle = 0; 
       private TransformComponent _cubeTransform;
       private TransformComponent _cubeTransform2;
       private TransformComponent _cubeTransform4;
       private ShaderEffectComponent cubeShader;
       private float colorswitch = 0;
       private float colorswitchLimit = 0;
    
     
        // Init is called on startup. 
        public override void Init()
        {
            // Set the clear color for the backbuffer to light green (intensities in R, G, B, A).
            RC.ClearColor = ColorUint.Tofloat4(ColorUint.Yellow);

            // Create a scene with a cube
            // The three components: one XForm, one Shader and the Mesh
            _cubeTransform = new TransformComponent {Scale = new float3(1, 1, 1), Translation = new float3(0, 0, 0)};
            cubeShader = new ShaderEffectComponent
            { 
                Effect = SimpleMeshes.MakeShaderEffect(new float3 (0, 0, 1),new float3 (1, 1, 1),  4)
            };
            var cubeMesh = SimpleMeshes.CreateCuboid(new float3(10, 10, 10));

              _cubeTransform2 = new TransformComponent {Scale = new float3(1, 1, 1), Translation = new float3(0, 0, 0)};
            var cubeShader2 = new ShaderEffectComponent
            { 
                Effect = SimpleMeshes.MakeShaderEffect(new float3 (1, 1, 1),new float3 (1, 1, 1),  4)
            };
            var cubeMesh2 = SimpleMeshes.CreateCuboid(new float3(30, 90, 10)); 

              _cubeTransform4 = new TransformComponent {Scale = new float3(1, 1, 1), Translation = new float3(0, 0, 0)};
            var cubeShader4 = new ShaderEffectComponent
            { 
                Effect = SimpleMeshes.MakeShaderEffect(new float3 (0, 1, 0),new float3 (1, 1, 1),  4)
            };
            var cubeMesh4 = SimpleMeshes.CreateCuboid(new float3(10, 10, 10)); 




            // Assemble the cube node containing the three components
            var cubeNode = new SceneNodeContainer();
            cubeNode.Components = new List<SceneComponentContainer>();
            cubeNode.Components.Add(_cubeTransform);
            cubeNode.Components.Add(cubeShader);
            cubeNode.Components.Add(cubeMesh);

            var cubeNode2 = new SceneNodeContainer();
            cubeNode2.Components = new List<SceneComponentContainer>();
            cubeNode2.Components.Add(_cubeTransform2);
            cubeNode2.Components.Add(cubeShader2);
            cubeNode2.Components.Add(cubeMesh2);

            var cubeNode4 = new SceneNodeContainer();
            cubeNode4.Components = new List<SceneComponentContainer>();
            cubeNode4.Components.Add(_cubeTransform4);
            cubeNode4.Components.Add(cubeShader4);
            cubeNode4.Components.Add(cubeMesh4);
            
        
            // Create the scene containing the cube as the only object
            _scene = new SceneContainer();
            _scene.Children = new List<SceneNodeContainer>();
            _scene.Children.Add(cubeNode);
            _scene.Children.Add(cubeNode2);
            _scene.Children.Add(cubeNode4);

            // Create a scene renderer holding the scene above
            _sceneRenderer = new SceneRenderer(_scene);
           
        }
        float gCI = 0.005f;
        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
           
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.ClearColor = new float4(RC.ClearColor.r,RC.ClearColor.g + gCI, RC.ClearColor.b, RC.ClearColor.a);
            if(RC.ClearColor.g >= 1 || RC.ClearColor.g <= 0)
            gCI = -gCI;
        

            // Animate the cube
            _cubeTransform.Translation = new float3(-100, 5 * M.Sin(3 * TimeSinceStart), 0);
            _cubeTransform2.Translation = new float3(0, 8 * M.Sin(5 * TimeSinceStart), 0);
            _cubeTransform4.Translation = new float3(100, 3 * M.Sin(8 * TimeSinceStart), 0);
            _cubeTransform4.Scale = new float3(5, 5 * M.Sin(TimeSinceStart), 5);


            if(colorswitch >= 1){
                colorswitchLimit = -0.01f;
            }
            if(colorswitch <= 0){
                colorswitchLimit = 0.01f;
            }
            colorswitch =colorswitch+colorswitchLimit;
            cubeShader.Effect = SimpleMeshes.MakeShaderEffect(new float3(colorswitch,0,1.0f-colorswitch),new float3(0,0,0),0);

            // Animate the camera angle
            //_camAngle = _camAngle + 90.0f * M.Pi/180.0f * DeltaTime;

            // Setup the camera 
            RC.View = float4x4.CreateTranslation(0, 0, 250) * float4x4.CreateRotationY(_camAngle);

            // Render the scene on the current render context
            _sceneRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }


        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            RC.Projection = projection;
        }
    }
}