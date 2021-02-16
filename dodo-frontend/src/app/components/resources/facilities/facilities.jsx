import React from 'react';
import { Icon } from 'app/components/icon';
import { Panel } from 'app/components/resources';
import { useTranslation } from "react-i18next";

import styles from './facilities.module.scss';

function camelToSentenceCase(text) {
	const sentence = text.replace( /([A-Z])/g, " $1" );
	return sentence.charAt(0).toUpperCase() + sentence.slice(1);
}

const icons = {
	toilets: 'toilet',
	bathrooms: 'bath',
	food: 'utensils',
	kitchen: 'sink',
	disabilityFriendly: 'wheelchair',
	outdoorCamping: 'campground',
	indoorCamping: 'warehouse',
	accomodation: 'bed',
	inductions: 'map-signs',
	talksAndTraining: 'comment',
	welfare: 'first-aid',
	affinityGroupFormation: 'hands-helping',
	volunteersNeeded: 'hand-paper',
	familyFriendly: 'baby-carriage',
	internet: 'wifi',
	electricity: 'plug',
	parking: 'parking',
};

export const Facilities = ({ facilities = {} }) => {
	const { t } = useTranslation("ui");
	const facilityKeys = Object.keys(facilities).filter(key => {
		return facilities[key] !== false && facilities[key] !== 'None';
	});

	return (
		<Panel>
			{!facilityKeys.length ? (
				<p>{t('This event offers no facilities')}.</p>
			)
			: (
				<ul className={styles.facilities}>
					{facilityKeys.map(key => {
						const value = facilities[key];
						const text = camelToSentenceCase(key);

						return (
							<li className={styles.facility} key={key}>
								<div className={styles.title}>
									{text}
									<Icon icon={icons[key]} title={text} className={styles.icon} />
								</div>
								<div className={`${styles.value} ${styles[value.toString().toLowerCase()]}`}>
									{value === true ? t('Yes') : value}
								</div>
							</li>
						)
					})}
				</ul>
			)}
		</Panel>
	);
};

export default Facilities;
