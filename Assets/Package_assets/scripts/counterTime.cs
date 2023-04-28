using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class counterTime : MonoBehaviour {


	public Light sun_light, light_shadower;
	private float r, g, b;
	private Color sun_light_color;

    private float auxIntensity, auxShadowStrength;

	private int minuto, hora, dia, mes, anyo;
	private int hora_aux, minuto_aux;

    private int tick;
    public int day_ini, month_ini, year_ini;
    public int timeMultiplierVelocity;
    private int aux_timeMultiplier;
        
    public Text date_time;

    public Color sunrise, early_morning, morning, midday, afternoon, evening, sunset, night;

	// Use this for initialization
	void Start () {
        tick = 0;
		minuto = 00;
		minuto_aux = -1;
		hora = 00;
		hora_aux = 00;
        dia = day_ini;
        mes = month_ini;
        anyo = year_ini;

        if (timeMultiplierVelocity >= 0) {
            aux_timeMultiplier = timeMultiplierVelocity;
            InvokeRepeating("newTick", 0, 1f / aux_timeMultiplier);
        } else {
            InvokeRepeating("newTick", 0, 1f);
        }
        
	}

    void Update() {
        if (timeMultiplierVelocity != aux_timeMultiplier && timeMultiplierVelocity>=0) {
            aux_timeMultiplier = timeMultiplierVelocity;
            CancelInvoke("newTick");
            InvokeRepeating("newTick", 0, 1f / aux_timeMultiplier);
        }
    }

	void FixedUpdate () {

        //nuevo minuto
        minuto = (int)(tick) - (59 * hora_aux);

		if(minuto>59){
            //nueva hora
			minuto=00;
			hora += 1;
			hora_aux += 1;

			if(hora>23){
                //nuevo dia
				hora=00;
				dia += 1;

                if (dia > 30) {
                    //nuevo mes
                    dia = 01;
                    mes += 1;

                    if (mes > 12) {
                        //nuevo anyo
                        mes = 01;
                        anyo += 1;
                    }
                }
			}
		}

		if(hora>=11 && hora<18){
            r = midday.r;
            g = midday.g;
            b = midday.b;
		}else if(hora>=18 && hora<19){
            r = afternoon.r;
            g = afternoon.g;
            b = afternoon.b;
		}else if(hora>=19 && hora<20){
            r = evening.r;
            g = evening.g;
            b = evening.b;
		}else if(hora>=20 && hora<21){
            r = sunset.r;
            g = sunset.g;
            b = sunset.b;
		}else if(hora>=21 || hora<6){
            r = night.r;
            g = night.g;
            b = night.b;
		}else if(hora>=6 && hora<7){
            r = sunrise.r;
            g = sunrise.g;
            b = sunrise.b;
		}else if(hora>=7 && hora<9){
            r = early_morning.r;
            g = early_morning.g;
            b = early_morning.b;
		}else if(hora>=9 && hora<11){
            r = morning.r;
            g = morning.g;
            b = morning.b;
		}


        if (hora >= 21 || hora < 6) {
            light_shadower.intensity = Mathf.Lerp(light_shadower.intensity, 0.07f, timeMultiplierVelocity / 900f);
            light_shadower.shadowStrength = Mathf.Lerp(light_shadower.shadowStrength, 0, timeMultiplierVelocity / 900f);
        } else {
            light_shadower.intensity = Mathf.Lerp(light_shadower.intensity, 0.59f, timeMultiplierVelocity / 300f);
            light_shadower.shadowStrength = Mathf.Lerp(light_shadower.shadowStrength, 1f, timeMultiplierVelocity / 300f);
        }


        sun_light.color = Color.Lerp(sun_light.color, new Color(r, g, b, 1f), (timeMultiplierVelocity/900f));
        light_shadower.color = Color.Lerp(light_shadower.color, new Color(r, g, b, 1f), (timeMultiplierVelocity/900f));
	
		if(minuto_aux != minuto){
			date_time.text = dia.ToString("00")+"/"+mes.ToString("00")+"/"+anyo+"\n"+hora.ToString("00")+":"+minuto.ToString("00");
			minuto_aux = minuto;
		}
	}

    private void newTick() {
        this.tick++;
    }

    public int getTick() {
        return this.tick;
    }

    public void setTimeMultiplier(int timeMultiplier) {
        if (timeMultiplier >= 0) {
            this.timeMultiplierVelocity = timeMultiplier;
        }
    }
}
