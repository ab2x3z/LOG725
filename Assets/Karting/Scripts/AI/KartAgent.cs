using KartGame.KartSystems;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using Random = UnityEngine.Random;
using Cinemachine.Utility;

namespace KartGame.AI
{
    /// <summary>
    /// Sensors hold information such as the position of rotation of the origin of the raycast and its hit threshold
    /// to consider a "crash".
    /// </summary>
    [System.Serializable]
    public struct Sensor
    {
        public Transform Transform;
        public float RayDistance;
        public float HitValidationDistance;
    }

    /// <summary>
    /// We only want certain behaviours when the agent runs.
    /// Training would allow certain functions such as OnAgentReset() be called and execute, while Inferencing will
    /// assume that the agent will continuously run and not reset.
    /// </summary>
    public enum AgentMode
    {
        Training,
        Inferencing
    }

    /// <summary>
    /// The KartAgent will drive the inputs for the KartController.
    /// </summary>
    public class KartAgent : Agent, IInput
    {
#region Training Modes
        [Tooltip("Are we training the agent or is the agent production ready?")]
        public AgentMode Mode = AgentMode.Training;

#endregion

#region Senses
        [Header("Observation Params")]
        [Tooltip("What objects should the raycasts hit and detect?")]
        public LayerMask Mask;
        [Tooltip("Sensors contain ray information to sense out the world, you can have as many sensors as you need.")]
        public Sensor[] Sensors = new Sensor[0];
        [Header("Checkpoints"), Tooltip("What is the (parent) list of checkpoints for the agent to seek and pass through?")]
        public GameObject CheckpointsList;

        private Collider[] Colliders = new Collider[0];

        [Space]
        [Tooltip("Would the agent need a custom transform to be able to raycast and hit the track? " +
            "If not assigned, then the root transform will be used.")]
        public Transform AgentSensorTransform;
#endregion

#region Rewards
        [Header("Rewards"), Tooltip("What penatly is given when the agent crashes?")]
        public float HitPenalty = -1f;
        [Tooltip("How much reward is given when the agent successfully passes the checkpoints?")]
        public float PassCheckpointReward;
        [Tooltip("How much penalty is given when the agent passes a wrong checkpoint?")]
        public float FailCheckpointPenalty;
        [Tooltip("How much penalty is given when the agent goes Out of bounds ?")]
        public float OOBPenalty;
        [Tooltip("Should typically be a small value, but we reward the agent for moving in the right direction.")]
        public float TowardsCheckpointReward;
        [Tooltip("Typically if the agent moves faster, we want to reward it for finishing the track quickly.")]
        public float SpeedReward;
        [Tooltip("Reward the agent when it keeps accelerating")]
        public float AccelerationReward;
        #endregion

        #region ResetParams
        [Header("Inference Reset Params")]
        [Tooltip("What is the unique mask that the agent should detect when it falls out of the track?")]
        public LayerMask OutOfBoundsMask;
        [Tooltip("What are the layers we want to detect for the track and the ground?")]
        public LayerMask TrackMask;
        [Tooltip("How far should the ray be when casted? For larger karts - this value should be larger too.")]
        public float GroundCastDistance;
#endregion

#region Debugging
        [Header("Debug Option")] [Tooltip("Should we visualize the rays that the agent draws?")]
        public bool ShowRaycasts;
#endregion

        ArcadeKart m_Kart;
        bool m_Acceleration;
        bool m_Brake;
        float m_Steering;

        bool m_EndEpisode;
        float m_LastAccumulatedReward;
        float m_CooldownCheckSensors;
        float EPSILON = 0.01f;

        void Awake()
        {
            m_Kart = GetComponent<ArcadeKart>();
            ArcadeKart.GoodCPSignal += HandleGoodCPSignal;
            if (AgentSensorTransform == null) AgentSensorTransform = transform;
            m_CooldownCheckSensors = -1.0f;

            if (CheckpointsList == null)
            {
                CheckpointsList = GameObject.Find("Checkpoints");
            }
            Colliders = new Collider[CheckpointsList.transform.childCount];
            for (int i = 0; i < CheckpointsList.transform.childCount; i++)
            {
                Colliders[i] = CheckpointsList.transform.GetChild(i).GetComponent<Collider>();
            }
        }

        void Start()
        {
            // If the agent is training, then at the start of the simulation, pick a random checkpoint to train the agent.
            OnEpisodeBegin();
        }

        void Update()
        {
            if (m_EndEpisode)
            {
                m_EndEpisode = false;
                AddReward(m_LastAccumulatedReward);
                EndEpisode();
                OnEpisodeBegin();
            }
            m_CooldownCheckSensors -= Time.deltaTime;

        }

        void LateUpdate()
        {
           
        }

        void HandleGoodCPSignal()
        {
            AddReward(PassCheckpointReward);
        }

        void OnDestroy()
        {
            ArcadeKart.GoodCPSignal -= HandleGoodCPSignal;
        }


        float Sign(float value)
        {
            if (value > 0)
            {
                return 1;
            } 
            if (value < 0)
            {
                return -1;
            }
            return 0;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(m_Kart.LocalSpeed());

            // Add an observation for direction of the agent to the next checkpoint.
            if(Colliders != null && Colliders.Length > 0)
            {
                var next = (Colliders.Length != 0) ? (m_Kart.m_CheckpointIndex + 1) % (Colliders.Length-1) : 0;
                var nextCollider = Colliders[next];
                if (nextCollider == null)
                    return;

                var direction = (nextCollider.transform.position - m_Kart.transform.position).normalized;
                sensor.AddObservation(Vector3.Dot(m_Kart.Rigidbody.velocity.normalized, direction));

                if (ShowRaycasts)
                    Debug.DrawLine(AgentSensorTransform.position, nextCollider.transform.position, Color.magenta);

                m_LastAccumulatedReward = 0.0f;
                m_EndEpisode = false;
                for (var i = 0; i < Sensors.Length; i++)
                {
                    var current = Sensors[i];
                    var xform = current.Transform;
                    var hit = Physics.Raycast(AgentSensorTransform.position, xform.forward, out var hitInfo,
                        current.RayDistance, Mask, QueryTriggerInteraction.Ignore);

                    if (ShowRaycasts)
                    {
                        Debug.DrawRay(AgentSensorTransform.position, xform.forward * current.RayDistance, Color.green);
                        Debug.DrawRay(AgentSensorTransform.position, xform.forward * current.HitValidationDistance, 
                            Color.red);

                        if (hit && hitInfo.distance < current.HitValidationDistance)
                        {
                            Debug.DrawRay(hitInfo.point, Vector3.up * 3.0f, Color.blue);
                        }
                    }

                    if (hit)
                    {
                        //On verif la normale du hit pour eviter de respawn si on touche une route trop pentue
                        if (hitInfo.distance < current.HitValidationDistance && m_CooldownCheckSensors <= 0.0f
                            && Mathf.Abs(hitInfo.normal.y) <= 0.4f )
                        {
                            m_LastAccumulatedReward += HitPenalty;
                            m_EndEpisode = true;
                        }
                    }

                    sensor.AddObservation(hit ? hitInfo.distance : current.RayDistance);
                }

                sensor.AddObservation(m_Acceleration);
            }

        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            base.OnActionReceived(actions);
            InterpretDiscreteActions(actions);

            if (ShowRaycasts)
                Debug.DrawRay(transform.position, Vector3.down * GroundCastDistance, Color.cyan);

            // Find the next checkpoint when registering the current checkpoint that the agent has passed.
            if (Colliders.Length > 0 && !(Mathf.Abs(m_Kart.Rigidbody.velocity.magnitude - 0f) < EPSILON))
            {
                var next = (m_Kart.m_CheckpointIndex + 1) % Colliders.Length;
                var nextCollider = Colliders[next];
                var direction = (nextCollider.transform.position - m_Kart.transform.position).normalized;
                var reward = Vector3.Dot(m_Kart.Rigidbody.velocity.normalized, direction);

                if (ShowRaycasts) Debug.DrawRay(AgentSensorTransform.position, m_Kart.Rigidbody.velocity, Color.blue);

                // Add rewards if the agent is heading in the right direction
                AddReward(reward * TowardsCheckpointReward);
                AddReward((m_Acceleration && !m_Brake ? 1.0f : 0.0f) * AccelerationReward);
                AddReward(m_Kart.LocalSpeed() * SpeedReward);
                //Debug.Log($"Agent is moving : {reward * TowardsCheckpointReward}");

            }
        }

        public override void OnEpisodeBegin()
        {
            switch (Mode)
            {
                case AgentMode.Training:
                    m_Kart.m_CheckpointIndex = Random.Range(0, Colliders.Length - 1);
                    var collider = Colliders[m_Kart.m_CheckpointIndex];
                    transform.localRotation = collider.transform.rotation;
                    transform.position = collider.transform.position;
                    m_Kart.Rigidbody.velocity = default;
                    m_Acceleration = false;
                    m_Brake = false;
                    m_Steering = 0f;
                    m_CooldownCheckSensors = 0.0f;
                    break;
                default:
                    break;
            }
        }

        //heuristic for the agent up arrow for acceleration, down arrow for brake, left and right for steering
        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var actions = actionsOut.DiscreteActions;
            actions.Clear();
            if ((Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow)) || (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow)))
            {
                actions[0] = 1;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                actions[0] = 2;
            }
            else
            {
                actions[0] = 0;
            }
            if ( (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow) ) || (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) ) )
            {
                actions[1] = 1;
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                actions[1] = 2;
            }
            else 
            {
                actions[1] = 0;
            }
        }

        void InterpretDiscreteActions(ActionBuffers actions)
        {
            m_Steering = actions.DiscreteActions[0] - 1f;
            m_Acceleration = actions.DiscreteActions[1] == 2 ;
            m_Brake = actions.DiscreteActions[1] == 0;
        }

        public InputData GenerateInput()
        {
            return new InputData
            {
                Accelerate = m_Acceleration,
                Brake = m_Brake,
                TurnInput = m_Steering
            };
        }
    }
}
