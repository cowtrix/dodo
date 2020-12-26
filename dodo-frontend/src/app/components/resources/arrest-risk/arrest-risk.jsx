import React, { useState } from 'react';
import PropTypes from 'prop-types';
import styles from './arrest-risk.module.scss';
import { Icon } from 'app/components'

const text = {
	low: 'A low arrest risk means there is or will be law enforcement present. Rebels should stay informed about the ongoing arrest risk, but engagement with law enforcement is unlikely.',
	moderate: 'A moderate arrest risk means that the organiser anticipates or has already experienced a small number of arrests, and that rebels may find themselves in arrestable situations. However, protesters will generally not be arrested without warning or without cause.',
	high: 'A high arrest risk means that law enforcement is actively making arrests or engaging in crowd suppression at this site. Rebels who are not prepared to be in high-risk situations should not come to this location.'
};

export const ArrestRiskLabel = ({ level, children }) => {
	if(level === 'high') {
		return <strong className={styles.label}>{children}</strong>;
	}
	return <span className={styles.label}>{children}</span>;
}

export const ArrestRisk = (props) => {
	const [showText, setShowText] = useState(false);

	const handleToggle = () => {
		setShowText(!showText);
	};

	if(!props.level || props.level === 'none') return null;

	return (
		<aside className={`${styles.arrestRisk} ${styles[props.level]}`}>
			<div className={styles.top}>
				<ArrestRiskLabel level={props.level}>
					Arrest risk: <span className={styles.level}>{props.level}</span>
				</ArrestRiskLabel>
				<button className={styles.toggle} onClick={handleToggle}>
					What does this mean?
					<Icon
						className={`${styles.icon} ${showText ? styles.hide : styles.show}`}
						icon='chevron-right'
						size="1x"
					/>
				</button>
			</div>
			{showText && (
				<div className={styles.text}>
					{text[props.level]}
				</div>
			)}
		</aside>
	);
}

ArrestRisk.propTypes= {
	level: PropTypes.oneOf(['none', 'low', 'moderate', 'high'])
}

export default ArrestRisk;
